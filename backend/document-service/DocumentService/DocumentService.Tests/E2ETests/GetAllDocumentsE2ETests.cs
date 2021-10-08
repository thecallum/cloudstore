using AutoFixture;
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
    public class GetAllDocumentsE2ETests : BaseIntegrationTest
    {
        public GetAllDocumentsE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
        }

        [Fact]
        public async Task GetAllDocuments_WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var query = new GetAllDocumentsQuery
            {
                DirectoryId = Guid.NewGuid()
            };

            // Act
            var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsNoDocuments()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            // Act
            var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDocumentsExist_ReturnsManyDocuments()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var numberOfDocuments = _random.Next(2, 5);
            var mockDocuments = _fixture
                .Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, userId)
                .CreateMany(numberOfDocuments);

            await SetupTestData(mockDocuments);

            // Act
            var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Documents.Should().HaveCount(numberOfDocuments);
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDirectoriesExist_ReturnsNoDirectories()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            // Act
            var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Directories.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDirectoriesExist_ReturnsManyDirectories()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var numberOfDirectories = _random.Next(2, 5);
            var mockDirectories = _fixture
                .Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, userId)
                .CreateMany(numberOfDirectories);

            await SetupTestData(mockDirectories);

            // Act
            var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Directories.Should().HaveCount(numberOfDirectories);
        }

        private async Task<HttpResponseMessage> GetAllDocumentsRequest(GetAllDocumentsQuery query)
        {
            var url = "/document-service/api/document";

            if (query.DirectoryId != null)  url += $"?directoryId={query.DirectoryId}";

            var uri = new Uri(url, UriKind.Relative);

            return await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }
    }
}
