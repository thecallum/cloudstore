using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
{
    public class S3GatewayTests : BaseIntegrationTest
    {
        private readonly IAmazonS3 _s3Client;
        private IS3Gateway _s3Gateway;

        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        public S3GatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3Client = testFixture.S3Client;
            _s3Gateway = new S3Gateway(_s3Client);
        }

        [Fact]
        public async Task UploadDocument_WhenFilePathIsInvalid_ThrowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var mockRequest = new UploadDocumentRequest { FilePath = "nonExistentFile.jpg" };

            // Act
            Func<Task<DocumentUploadResponse>> func = async () => await _s3Gateway.UploadDocument(mockRequest, documentId, userId);

            // Assert
            await func.Should().ThrowAsync<InvalidFilePathException>();
        }

        [Fact]
        public async Task UploadDocument_WhenFileTooLarge_ThrowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var mockRequest = new UploadDocumentRequest { FilePath = _tooLargeFilePath };

            // Act
            Func<Task<DocumentUploadResponse>> func = async () => await _s3Gateway.UploadDocument(mockRequest, documentId, userId);

            // Assert
            await func.Should().ThrowAsync<FileTooLargeException>();
        }

        [Fact]
        public async Task UploadDocument_WhenValid_UploadsDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var mockRequest = new UploadDocumentRequest { FilePath = _validFilePath };

            // Act
            var response = await _s3Gateway.UploadDocument(mockRequest, documentId, userId);

            // Assert
            response.Should().NotBeNull();

            response.DocumentName.Should().Be("validfile.txt");
            response.S3Location.Should().Be($"{userId}/{documentId}");
            response.FileSize.Should().Be(200);

            await VerifyDocumentUploadedToS3(userId, documentId);
        }

        private async Task VerifyDocumentUploadedToS3(Guid userId, Guid documentId)
        {
            // test file exists
            var request = new GetObjectMetadataRequest
            {
                BucketName = "uploadfromcs",
                Key = $"{userId}/{documentId}"
            };

            try
            {
                await _s3Client.GetObjectMetadataAsync(request);
            } catch(Exception)
            {
                throw new Exception("Document Metadata could lot be loaded from s3");
            }
        }
    }
}
