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
using DocumentService.Tests.Helpers;

namespace DocumentService.Tests.E2ETests
{
    public class GetDocumentDownloadLinkE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public GetDocumentDownloadLinkE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            // Act
            var response = await GetDocumentDownloadLinkRequest(documentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDocumentExists_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var document = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, userId)
                .Create();

            await SetupTestData(document);

            await _s3TestHelper.UploadDocumentToS3($"store/{document.S3Location}", _validFilePath);

            // Act
            var response = await GetDocumentDownloadLinkRequest(document.DocumentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetDocumentLinkResponse>(response);

            responseContent.DocumentLink.Should().NotBeNull();
            responseContent.DocumentLink.Should().Contain("AWSAccessKeyId");
        }

        private async Task<HttpResponseMessage> GetDocumentDownloadLinkRequest(Guid documentId)
        {
            var uri = new Uri($"/document-service/api/document/download/{documentId}", UriKind.Relative);

            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return response;
        }
    }
}
