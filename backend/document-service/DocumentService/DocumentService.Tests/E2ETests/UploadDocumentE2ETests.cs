using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using Amazon.S3.Model;
using DocumentService.Tests.Helpers;

namespace DocumentService.Tests.E2ETests
{
    public class UploadDocumentE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public UploadDocumentE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = "nonExistantFile.txt",
                DirectoryId = Guid.NewGuid()
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UploadDocument_WhenInvalidFilePath_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = "nonExistantFile.txt",
                DirectoryId = null
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UploadDocument_WhenFileTooLarge_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _tooLargeFilePath,
                DirectoryId = null
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryIdIsNull_SetsDirectoryIdAsUserId()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _validFilePath,
                DirectoryId = null
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);


            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<UploadDocumentResponse>(responseContent, CreateJsonOptions());

            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var expectedS3Location = $"{userId}/{uploadDocumentResponse.DocumentId}";
            var expectedFileName = "validfile.txt";

            uploadDocumentResponse.Name.Should().Be(expectedFileName);
            uploadDocumentResponse.S3Location.Should().Be(expectedS3Location);

            // will be set intoken
            var databaseResponse = await _context.LoadAsync<DocumentDb>(userId, uploadDocumentResponse.DocumentId);
            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(userId);
            databaseResponse.Name.Should().Be(expectedFileName);
            databaseResponse.S3Location.Should().Be(expectedS3Location);
            databaseResponse.FileSize.Should().Be(200);
            databaseResponse.DirectoryId.Should().Be(userId);

            var expectedDocumentKey = $"{userId}/{uploadDocumentResponse.DocumentId}";

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(userId, databaseResponse.DocumentId));
            _cleanup.Add(async () => await _s3TestHelper.DeleteDocumentFromS3(expectedDocumentKey));
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryIdIsNotNull_SetsDirectoryIdDirectoryId()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var mockDirectory = _fixture.Build<DirectoryDb>().With(x => x.UserId, userId).Create();
            await SetupTestData(mockDirectory);

            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _validFilePath,
                DirectoryId = mockDirectory.DirectoryId
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);


            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<UploadDocumentResponse>(responseContent, CreateJsonOptions());

            var expectedS3Location = $"{userId}/{uploadDocumentResponse.DocumentId}";
            var expectedFileName = "validfile.txt";

            uploadDocumentResponse.Name.Should().Be(expectedFileName);
            uploadDocumentResponse.S3Location.Should().Be(expectedS3Location);

            // will be set intoken
            var databaseResponse = await _context.LoadAsync<DocumentDb>(userId, uploadDocumentResponse.DocumentId);
            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(userId);
            databaseResponse.Name.Should().Be(expectedFileName);
            databaseResponse.S3Location.Should().Be(expectedS3Location);
            databaseResponse.FileSize.Should().Be(200);
            databaseResponse.DirectoryId.Should().Be(mockDirectory.DirectoryId);

            var expectedDocumentKey = $"{userId}/{uploadDocumentResponse.DocumentId}";

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(userId, databaseResponse.DocumentId));
            _cleanup.Add(async () => await _s3TestHelper.DeleteDocumentFromS3(expectedDocumentKey));
        }

        [Fact]
        public async Task UploadDocument_WhenValid_UploadsDocumentToS3()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _validFilePath,
                DirectoryId = null
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<UploadDocumentResponse>(responseContent, CreateJsonOptions());

            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var expectedDocumentKey = $"{userId}/{uploadDocumentResponse.DocumentId}";

            await _s3TestHelper.VerifyDocumentUploadedToS3(expectedDocumentKey);

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(userId, uploadDocumentResponse.DocumentId));
            _cleanup.Add(async () => await _s3TestHelper.DeleteDocumentFromS3(expectedDocumentKey));
        }

        private async Task<HttpResponseMessage> UploadDocumentRequest(UploadDocumentRequest request)
        {
            var uri = new Uri("/api/document/upload", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}
