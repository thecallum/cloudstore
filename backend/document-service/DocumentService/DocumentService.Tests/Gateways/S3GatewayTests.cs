using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using DocumentService.Tests.Helpers;

namespace DocumentService.Tests.Gateways
{
    public class S3GatewayTests : BaseIntegrationTest
    {
        private readonly IS3Gateway _s3Gateway;

        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public S3GatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3Gateway = new S3Gateway(_s3Client);
            _s3TestHelper = new S3TestHelper(_s3Client);
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

            await _s3TestHelper.VerifyDocumentUploadedToS3($"{userId}/{documentId}");
        }

        [Fact]
        public async Task GetDocumentPresignedUrl_WhenCalled_ReturnsValidUrl()
        {
            // Arrange
            var key = _fixture.Create<string>();

            await _s3TestHelper.UploadDocumentToS3(key, _validFilePath);

            // Act
            var response = _s3Gateway.GetDocumentPresignedUrl(key);

            // Assert
            response.Should().NotBeNull();
            response.Should().Contain("AWSAccessKeyId");
        }

        [Fact]
        public async Task DeleteDocument_WhenCalled_DeletesDocument()
        {
            // Arrange
            var key = _fixture.Create<string>();

            await _s3TestHelper.UploadDocumentToS3(key, _validFilePath);

            // Act
            await _s3Gateway.DeleteDocument(key);

            // Assert
            await _s3TestHelper.VerifyDocumentDeletedFromS3(key);
        }
    }
}
