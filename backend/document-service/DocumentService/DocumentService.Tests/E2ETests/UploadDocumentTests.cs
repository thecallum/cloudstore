using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class UploadDocumentTests : BaseIntegrationTest
    {
        private string _validFilePath;
        private string _tooLargeFilePath;

        public UploadDocumentTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;
        }

        [Fact]
        public async Task UploadDocument_WhenInvalidFilePath_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = "nonExistantFile.txt"
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
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task UploadDocument_WhenValid_ReturnsUploadDocumentResponse()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _validFilePath
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

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(userId, databaseResponse.DocumentId));
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
