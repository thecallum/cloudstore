using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
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

            var numberOfDirectories = _random.Next(2, 5);
            var mockDirectories = _fixture
                .Build<DirectoryDb>()
                .With(x => x.UserId, _userId)
                .With(x => x.ParentDirectoryId, _userId)
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
            // setup request
            var url = "/document-service/api/directory";

            if (query.DirectoryId != null) url += $"?directoryId={query.DirectoryId}";

            var uri = new Uri(url, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Method = HttpMethod.Get;
            message.Headers.Add("authorizationToken", _token);

            //message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
