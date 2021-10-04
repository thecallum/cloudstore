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
using System.IO;
using Amazon.S3.Model;

namespace DocumentService.Tests.E2ETests
{
    public class GetDocumentLinkE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly string _bucketName = "uploadfromcs";


        public GetDocumentLinkE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;
        }

        [Fact]
        public async Task GetDocumentLink_WhenDocumentDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            // Act

            var response = await GetDocumentLinkRequest(documentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDocumentLink_WhenDocumentExists_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var document = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, userId)
                .Create();

            await SetupTestData(document);
            await UploadDocumentToS3(document.S3Location, _validFilePath);

            // Act
            var response = await GetDocumentLinkRequest(document.DocumentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var getDocumentLinkResponse = System.Text.Json.JsonSerializer.Deserialize<GetDocumentLinkResponse>(responseContent, CreateJsonOptions());

            getDocumentLinkResponse.DocumentLink.Should().NotBeNull();
            getDocumentLinkResponse.DocumentLink.Should().Contain("AWSAccessKeyId");
        }

        private async Task UploadDocumentToS3(string key, string filePath)
        {
            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var s3Request = new PutObjectRequest()
                {
                    InputStream = inputStream,
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.PutObjectAsync(s3Request);
            }
        }

        private async Task<HttpResponseMessage> GetDocumentLinkRequest(Guid documentId)
        {
            var uri = new Uri($"/api/document/{documentId}", UriKind.Relative);

            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return response;
        }
    }
}
