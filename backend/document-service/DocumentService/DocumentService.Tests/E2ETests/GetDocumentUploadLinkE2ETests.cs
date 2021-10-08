using DocumentService.Boundary.Response;
using DocumentService.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class GetDocumentUploadLinkE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public GetDocumentUploadLinkE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task GetDocumentUploadLink_WhenCalled_UploadsFileToUploadDirectory()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            // Act
            var response = await GetDocumentUploadLinkRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetDocumentUploadResponse>(response);

            // test presigned url
            _s3TestHelper.TestUploadWithPresignedUrl(responseContent.UploadUrl, _validFilePath);

            var key = $"{userId}/{responseContent.DocumentId}";

            await _s3TestHelper.VerifyDocumentUploadedToS3($"upload/{key}");
        }

        private async Task<HttpResponseMessage> GetDocumentUploadLinkRequest()
        {
            var uri = new Uri($"/document-service/api/document/upload", UriKind.Relative);

            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return response;
        }
    }
}
