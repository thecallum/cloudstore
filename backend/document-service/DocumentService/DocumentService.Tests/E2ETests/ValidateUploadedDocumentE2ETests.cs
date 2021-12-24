using DocumentService.Boundary.Request;
using DocumentService.Tests.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;
using System.Net;
using DocumentService.Domain;
using DocumentService.Infrastructure;
using System.Net.Http.Headers;
using DocumentService.Factories;
using DocumentService.Boundary.Response;

namespace DocumentService.Tests.E2ETests
{
    public class ValidateUploadedDocumentE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;
        private readonly S3TestHelper _s3TestHelper;

        public ValidateUploadedDocumentE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;
            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDocumentTooLarge_Returns413()
        {
            // Arrange
            var user = ContextHelper.CreateUser(200);
            var token = ContextHelper.CreateToken(user);

            var documentId = Guid.NewGuid();
            var key = $"{user.Id}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _tooLargeFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request, token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.RequestEntityTooLarge);
        }

        [Fact]
        public async Task WhenDocumentExists_ReturnsDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var key = $"{_user.Id}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            response.Headers.Location.ToString().Should().Contain(documentId.ToString());

            var responseContent = await DecodeResponse<DocumentResponse>(response);

            responseContent.Should().NotBeNull();
            responseContent.Id.Should().Be(documentId);
            responseContent.DirectoryId.Should().Be((Guid)request.DirectoryId);
            responseContent.FileSize.Should().Be(200);
            responseContent.Name.Should().Be(request.FileName);
            responseContent.S3Location.Should().Be($"{_user.Id}/{documentId}");
        }

        [Fact]
        public async Task WhenDocumentExists_MovesDocumentToStoreDirectory()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var key = $"{_user.Id}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"store/{key}");
            await _s3TestHelper.VerifyDocumentDeletedFromS3($"upload/{key}");
        }

        [Fact]
        public async Task WhenNewDocument_InsertsDocumentIntoDatabase()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var key = $"{_user.Id}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            var databaseResponse = await _dbFixture.GetDocumentById(documentId);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Id.Should().Be(documentId);
            databaseResponse.UserId.Should().Be(_user.Id);
            databaseResponse.DirectoryId.Should().Be((Guid)request.DirectoryId);
            databaseResponse.FileSize.Should().Be(200);
            databaseResponse.Name.Should().Be(request.FileName);
            databaseResponse.S3Location.Should().Be($"{_user.Id}/{documentId}");
        }

        [Fact]
        public async Task WhenExistingDocument_UpdatesDocumentIntoDatabase()
        {
            // Arrange
            var existingDocument = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, _user.Id)
                .Create()
                .ToDatabase();

            await _dbFixture.SetupTestData(existingDocument);

            var key = $"{_user.Id}/{existingDocument.Id}";

            var request = _fixture.Create<ValidateUploadedDocumentRequest>();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(existingDocument.Id, request);

            // Assert
            var databaseResponse = await _dbFixture.GetDocumentById(existingDocument.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Id.Should().Be(existingDocument.Id);
            databaseResponse.UserId.Should().Be(_user.Id);
            databaseResponse.DirectoryId.Should().Be(existingDocument.DirectoryId);
            databaseResponse.FileSize.Should().Be(200);
            databaseResponse.Name.Should().Be(existingDocument.Name);
            databaseResponse.S3Location.Should().Be(existingDocument.S3Location);
        }

        private async Task<HttpResponseMessage> ValidateUploadedDocumentRequest(Guid documentId, ValidateUploadedDocumentRequest request, string? token = null)
        {
            // setup request
            var uri = new Uri($"api/document/validate/{documentId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Method = HttpMethod.Post;
            message.Headers.Add(TokenService.Constants.AuthToken, token ?? _token);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
