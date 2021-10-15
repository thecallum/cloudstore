using DocumentService.Gateways;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using DocumentService.Tests.Helpers;
using DocumentService.Gateways.Interfaces;

namespace DocumentService.Tests.Gateways
{
    public class S3GatewayTests : BaseIntegrationTest
    {
        private readonly IS3Gateway _s3Gateway;

        private readonly string _validFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public S3GatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;

            _s3Gateway = new S3Gateway(_s3Client);
            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task GetDocumentDownloadLink_WhenCalled_ReturnsValidUrl()
        {
            // Potentially implement test on downloading the file

            // Arrange
            var key = _fixture.Create<string>();
            var fileName = "file.txt";

            await _s3TestHelper.UploadDocumentToS3(key, _validFilePath);

            // Act
            var response = _s3Gateway.GetDocumentDownloadPresignedUrl(key, fileName);

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

        [Fact]
        public async Task GetDocumentUploadPresignedUrl_WhenCalled_EnablesFileUploadToS3()
        {
            // Arrange
            var key = _fixture.Create<string>();

            // Act
            var uploadUrl = _s3Gateway.GetDocumentUploadPresignedUrl(key);

            _s3TestHelper.TestUploadWithPresignedUrl(uploadUrl, _validFilePath);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"upload/{key}");
        }

        [Fact]
        public async Task ValidateUploadedDocument_WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var key = _fixture.Create<string>();

            // Act
            var result = await _s3Gateway.ValidateUploadedDocument(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ValidateUploadedDocument_WhenDocumentExists_ReturnsFileSize()
        {
            // Arrange
            var key = _fixture.Create<string>();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var result = await _s3Gateway.ValidateUploadedDocument(key);

            // Assert
            result.Should().NotBeNull();
            result.FileSize.Should().Be(200);
        }

        [Fact]
        public async Task MoveDocumentToStoreDirectory_WhenCalled_MovesDocumentToStoreDirectory()
        {
            // Arrange
            var key = _fixture.Create<string>();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"store/{key}");
        }

        [Fact]
        public async Task MoveDocumentToStoreDirectory_WhenCalled_DeletesDocumentFromUploadDirectory()
        {
            // Arrange
            var key = _fixture.Create<string>();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            // Assert
            await _s3TestHelper.VerifyDocumentDeletedFromS3($"upload/{key}");
        }
    }
}
