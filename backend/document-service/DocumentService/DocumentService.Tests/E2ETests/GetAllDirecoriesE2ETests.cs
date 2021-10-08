using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
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
    public class GetAllDirecoriesE2ETests : BaseIntegrationTest
    {

        public GetAllDirecoriesE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
        }

        [Fact]
        public async Task WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var query = new GetAllDirectoriesQuery
            {
                DirectoryId = Guid.NewGuid()
            };

            // Act
            var response = await GetAllDirectoriesRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenNoDirectoriesExist_ReturnsNoDirectories()
        {
            // Arrange
            var query = new GetAllDirectoriesQuery { DirectoryId = null };

            // Act
            var response = await GetAllDirectoriesRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetAllDirectoriesResponse>(response);

            responseContent.Directories.Should().HaveCount(0);
        }

        [Fact]
        public async Task WhenManyDirectoriesExist_ReturnsManyDirectories()
        {
            // Arrange
            var query = new GetAllDirectoriesQuery { DirectoryId = null };
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var numberOfDirectories = _random.Next(2, 5);
            var mockDirectories = _fixture
                .Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, userId)
                .CreateMany(numberOfDirectories);

            await SetupTestData(mockDirectories);

            // Act
            var response = await GetAllDirectoriesRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await DecodeResponse<GetAllDirectoriesResponse>(response);

            responseContent.Directories.Should().HaveCount(numberOfDirectories);
        }

        private async Task<HttpResponseMessage> GetAllDirectoriesRequest(GetAllDirectoriesQuery query)
        {
            var url = "/document-service/api/directory";

            if (query.DirectoryId != null) url += $"?directoryId={query.DirectoryId}";

            var uri = new Uri(url, UriKind.Relative);

            return await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }
    }
}
