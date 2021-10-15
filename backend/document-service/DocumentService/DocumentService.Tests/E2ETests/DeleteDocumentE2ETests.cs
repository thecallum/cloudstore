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
using DocumentService.Tests.Helpers;
using System.Net.Http.Headers;

namespace DocumentService.Tests.E2ETests
{
    public class DeleteDocumentE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly S3TestHelper _s3TestHelper;

        public DeleteDocumentE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _s3TestHelper = new S3TestHelper(_s3Client);
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
            var document = _fixture.Build<DocumentDb>().With(x => x.UserId, _userId).Create();
            await SetupTestData(document);

            var storageUsage = new DocumentStorageDb { UserId = _userId, StorageUsage = document.FileSize };
            await SetupTestData(storageUsage);

            await _s3TestHelper.UploadDocumentToS3(document.S3Location, _validFilePath);

            // Act
            var response = await DeleteDocumentRequest(document.DocumentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var databaseResponse = await _context.LoadAsync<DocumentDb>(_userId, document.DocumentId);
            databaseResponse.Should().BeNull();

            await _s3TestHelper.VerifyDocumentDeletedFromS3(document.S3Location);

            // check storage service
            var databaseResponse2 = await _context.LoadAsync<DocumentStorageDb>(_userId);
            databaseResponse2.StorageUsage.Should().Be(0);
        }

        private async Task<HttpResponseMessage> DeleteDocumentRequest(Guid documentId)
        {
            // setup request
            var uri = new Uri($"/document-service/api/document/{documentId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Method = HttpMethod.Delete;
            message.Headers.Add("authorizationToken", _token);


            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);

        }
    }
}
