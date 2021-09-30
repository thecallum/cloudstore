using AutoFixture;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class GetAllDocumentsE2ETests : BaseIntegrationTest
    {
        public GetAllDocumentsE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsEmptyList()
        {
            // Arrange

            // Act
            var response = await GetAllDocumentsRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<GetAllDocumentsResponse>(responseContent, CreateJsonOptions());

            uploadDocumentResponse.Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDocumentsExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var numberOfDocuments = _random.Next(2, 5);
            var mockDocuments = _fixture
                .Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .CreateMany(numberOfDocuments);

            await SetupTestData(mockDocuments);

            // Act
            var response = await GetAllDocumentsRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<GetAllDocumentsResponse>(responseContent, CreateJsonOptions());

            uploadDocumentResponse.Documents.Should().HaveCount(numberOfDocuments);
        }

        private async Task<HttpResponseMessage> GetAllDocumentsRequest()
        {
            var uri = new Uri("/api/document", UriKind.Relative);

            //var json = JsonConvert.SerializeObject(request);
           // var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return response;
        }
    }
}
