using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using AutoFixture;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways.Exceptions;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Tests.Helpers;
using DocumentServiceListener.UseCase;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.E2ETests
{
    [Collection("Database collection")]
    public class DocumentUploadedE2ETests
    {
        private readonly S3Gateway _gateway;
        private readonly Fixture _fixture = new Fixture();
        private readonly S3TestHelper _s3TestHelper;
        private readonly string _validFilePath;
        private readonly ILambdaContext _lambdaContext;

        private readonly LambdaEntryPoint _lambdaEntryPoint;

        private readonly Func<ScopedContext> _getContext;

        public DocumentUploadedE2ETests(DatabaseFixture testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;

            _gateway = new S3Gateway(testFixture.S3Client);
            _s3TestHelper = new S3TestHelper(testFixture.S3Client);

            _getContext = testFixture.GetContext;


            _lambdaContext= new TestLambdaContext()
            {
                //Logger = mockLambdaLogger.Object
            };

            _lambdaEntryPoint = new LambdaEntryPoint();

        }

        private readonly static JsonSerializerOptions _jsonOptions = CreateJsonOptions();

        [Fact]
        public async Task WhenObjectNotFoundInS3_SwallowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MaxImageSize", "10000");

            // create mock document
            var document = _fixture.Create<DocumentDb>();

            var CSEvent = CreateSnsEvent(document.Id, document.UserId);
            var sqsEvent = CreateSQSEvent(CSEvent);

            // Act
            Func<Task> func = async () => await _lambdaEntryPoint.FunctionHandler(sqsEvent, _lambdaContext);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenImageTooLarge_SwallowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MaxImageSize", "10");

            var document = _fixture.Create<DocumentDb>();

            var CSEvent = CreateSnsEvent(document.Id, document.UserId);
            var objectKey = $"{CSEvent.User.Id}/{document.Id}";

            // upload valid image to S3
            var filePath = TestHelpers.GetFilePath("PlaceholderImage.png");
            await _s3TestHelper.UploadDocumentToS3($"store/{objectKey}", filePath);

            var sqsEvent = CreateSQSEvent(CSEvent);

            // Act
            Func<Task> func = async () => await _lambdaEntryPoint.FunctionHandler(sqsEvent, _lambdaContext);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenImageFormatInvalid_SwallowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MaxImageSize", "10000");

            var document = _fixture.Create<DocumentDb>();

            var CSEvent = CreateSnsEvent(document.Id, document.UserId);
            var objectKey = $"{CSEvent.User.Id}/{document.Id}";

            // upload valid image to S3
            var filePath = TestHelpers.GetFilePath("validfile.txt"); // not an image file
            await _s3TestHelper.UploadDocumentToS3($"store/{objectKey}", filePath);

            var sqsEvent = CreateSQSEvent(CSEvent);

            // Act
            Func<Task> func = async () => await _lambdaEntryPoint.FunctionHandler(sqsEvent, _lambdaContext);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenDocumentNotFound_ThrowsDocumentDbNotFoundException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MaxImageSize", "10000");

            var document = _fixture.Create<DocumentDb>();

            var CSEvent = CreateSnsEvent(document.Id, document.UserId);
            var objectKey = $"{CSEvent.User.Id}/{document.Id}";

            // upload valid image to S3
            var filePath = TestHelpers.GetFilePath("PlaceholderImage.png");
            await _s3TestHelper.UploadDocumentToS3($"store/{objectKey}", filePath);

            var sqsEvent = CreateSQSEvent(CSEvent);

            // Act
            Func<Task> func = async () => await _lambdaEntryPoint.FunctionHandler(sqsEvent, _lambdaContext);

            // Assert
            await func.Should().ThrowAsync<DocumentDbNotFoundException>();
        }

        [Fact]
        public async Task WhenCalled_GeneratesAndSavesThumbnailToS3()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MaxImageSize", "10000");

            var document = _fixture.Create<DocumentDb>();
            await SetupTestData(document);

            var CSEvent = CreateSnsEvent(document.Id, document.UserId);

            var objectKey = $"{CSEvent.User.Id}/{document.Id}";

            // upload valid image to S3
            var filePath = TestHelpers.GetFilePath("PlaceholderImage.png");
            await _s3TestHelper.UploadDocumentToS3($"store/{objectKey}", filePath);

            var sqsEvent = CreateSQSEvent(CSEvent);

            // Act
            await _lambdaEntryPoint.FunctionHandler(sqsEvent, _lambdaContext);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"thumbnails/{objectKey}");

            var databaseResponse = await LoadDocument(document.Id);
            databaseResponse.Should().NotBeNull();

            var expectedThumbnailUrl = $"https://uploadfromcs.s3.eu-west-1.amazonaws.com/thumbnails/{objectKey}";

            databaseResponse.Thumbnail.Should().Be(expectedThumbnailUrl);
        }

        private async Task SetupTestData(DocumentDb document)
        {
            using (var ctx = _getContext())
            {
                var db = ctx.DB;

                db.Documents.Add(document);
                await db.SaveChangesAsync();
            }
        }

        private CloudStoreSnsEvent CreateSnsEvent(Guid DocumentId, Guid userId)
        {
            var CSEvent = _fixture.Build<CloudStoreSnsEvent>()
               .With(x => x.EventName, EventNames.DocumentUploaded)
               .Create();

            CSEvent.Body.Add("DocumentId", DocumentId.ToString());
            CSEvent.User.Id = userId;

            return CSEvent;
        }

        private async Task<DocumentDb> LoadDocument(Guid documentId)
        {
            using (var ctx = _getContext())
            {
                var db = ctx.DB;

                return await db.Documents.FindAsync(documentId);
            }
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }



        private SQSEvent CreateSQSEvent(CloudStoreSnsEvent cloudStoreEvent)
        {
            var msgBody = JsonSerializer.Serialize(cloudStoreEvent, _jsonOptions);

            var SQSMessage = _fixture.Build<SQSEvent.SQSMessage>()
                   .With(x => x.Body, msgBody)
                   .With(x => x.MessageAttributes, new Dictionary<string, SQSEvent.MessageAttribute>())
                    .Create();

            var sqsEvent = _fixture.Build<SQSEvent>()
                    .With(x => x.Records, new List<SQSEvent.SQSMessage> { SQSMessage })
                    .Create();

            return sqsEvent;
        }
    }
}
