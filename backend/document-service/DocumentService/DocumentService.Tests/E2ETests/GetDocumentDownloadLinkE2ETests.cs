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
using System.Net.Http.Headers;
using DocumentService.Domain;
using DocumentService.Factories;

namespace DocumentService.Tests.E2ETests
{
    public class GetDocumentDownloadLinkE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public GetDocumentDownloadLinkE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;

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
            var document = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, _user.Id)
                .With(x => x.DirectoryId, (Guid?) null)
                .Create()
                .ToDatabase();

            await _dbFixture.SetupTestData(document);

            await _s3TestHelper.UploadDocumentToS3($"store/{document.S3Location}", _validFilePath);

            // Act
            var response = await GetDocumentDownloadLinkRequest(document.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetDocumentLinkResponse>(response);

            responseContent.DocumentLink.Should().NotBeNull();
            responseContent.DocumentLink.Should().Contain(document.S3Location);
        }

        private async Task<HttpResponseMessage> GetDocumentDownloadLinkRequest(Guid documentId)
        {
            // setup request
            var uri = new Uri($"/api/document/download/{documentId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Method = HttpMethod.Get;
            message.Headers.Add(TokenService.Constants.AuthToken, _token);

            //message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
