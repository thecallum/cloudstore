using DocumentService.Boundary.Response;
using DocumentService.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class GetDocumentUploadLinkE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly S3TestHelper _s3TestHelper;

        public GetDocumentUploadLinkE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task GetDocumentUploadLink_WhenCalled_UploadsFileToUploadDirectory()
        {
            // Arrange

            // Act
            var response = await GetDocumentUploadLinkRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetDocumentUploadResponse>(response);

            // test presigned url
            _s3TestHelper.TestUploadWithPresignedUrl(responseContent.UploadUrl, _validFilePath);

            var key = $"{_userId}/{responseContent.DocumentId}";

            await _s3TestHelper.VerifyDocumentUploadedToS3($"upload/{key}");
        }

        private async Task<HttpResponseMessage> GetDocumentUploadLinkRequest()
        {
            // setup request
            var uri = new Uri($"/document-service/api/document/upload", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Method = HttpMethod.Get;
            message.Headers.Add("authorizationToken", _token);

           // message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
