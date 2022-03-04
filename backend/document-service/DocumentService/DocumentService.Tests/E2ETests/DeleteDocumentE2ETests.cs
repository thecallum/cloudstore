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
using DocumentService.Domain;
using DocumentService.Factories;

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
            var document = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, _user.Id)
                .Create()
                .ToDatabase();

            await _dbFixture.SetupTestData(document);

            await _s3TestHelper.UploadDocumentToS3(document.S3Location, _validFilePath);

            // Act
            var response = await DeleteDocumentRequest(document.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var databaseResponse = await _dbFixture.GetDocumentById(document.Id);
            databaseResponse.Should().BeNull();

            await _s3TestHelper.VerifyDocumentDeletedFromS3(document.S3Location);
        }

        private async Task<HttpResponseMessage> DeleteDocumentRequest(Guid documentId)
        {
            // setup request
            var uri = new Uri($"/api/document/{documentId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Method = HttpMethod.Delete;
            message.Headers.Add("Authorization", _token);


            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);

        }
    }
}
