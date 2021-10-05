using Xunit;
using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Amazon.S3.Model;

namespace DocumentService.Tests.E2ETests
{
    public class DeleteDocumentE2ETests : BaseIntegrationTest
    {
        private readonly string _bucketName = "uploadfromcs";
        private readonly string _validFilePath;

        public DeleteDocumentE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            // Act
            var response = await DeleteDocumentRequest(documentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentFound_ReturnsNoContentResponse()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var document = _fixture.Build<DocumentDb>().With(x => x.UserId, userId).Create();
            await SetupTestData(document);

            await UploadDocumentToS3(document.S3Location);

            // Act
            var response = await DeleteDocumentRequest(document.DocumentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var databaseResponse = await _context.LoadAsync<DocumentDb>(userId, document.DocumentId);
            databaseResponse.Should().BeNull();

            await VerifyDocumentDeletedFromS3(document.S3Location);
        }

        private async Task UploadDocumentToS3(string key)
        {
            using (FileStream inputStream = new FileStream(_validFilePath, FileMode.Open, FileAccess.Read))
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

        

        private async Task<HttpResponseMessage> DeleteDocumentRequest(Guid documentId)
        {
            var uri = new Uri($"/api/document/{documentId}", UriKind.Relative);

            var response = await _httpClient.DeleteAsync(uri).ConfigureAwait(false);

            return response;
        }

        private async Task VerifyDocumentDeletedFromS3(string key)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = "uploadfromcs",
                Key = key
            };

            try
            {
                await _s3Client.GetObjectMetadataAsync(request);

                throw new Exception("Document still found in s3");
            }
            catch (Exception)
            {
                // should throw error
            }
        }
    }
}
