using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Infrastructure;
using DocumentService.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace DocumentService.Tests.E2ETests
{
    public class DeleteDirectoryE2ETests : BaseIntegrationTest
    {
        public DeleteDirectoryE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
        }

        [Fact]
        public async Task Delete_WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var directoryId = Guid.NewGuid();

            // Act
            var response = await DeleteDirectoryRequest(directoryId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsDocuments_ReturnsBadRequest()
        {
            // Arrange
            var directoryId = Guid.NewGuid();

            var document = _fixture.Build<DocumentDb>()
                            .With(x => x.UserId, _user.Id)
                            .With(x => x.DirectoryId, directoryId)
                            .Create();

            await SetupTestData(document);

            // Act
            var response = await DeleteDirectoryRequest(directoryId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsChildDirectories_ReturnsBadRequest()
        {
            // Arrange
            var parentDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, _user.Id)
                .With(x => x.ParentDirectoryId, _user.Id)
                .Create();

            await SetupTestData(parentDirectory);

            var childDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, _user.Id)
                .With(x => x.ParentDirectoryId, parentDirectory.DirectoryId)
                .Create();

            await SetupTestData(childDirectory);

            // Act
            var response = await DeleteDirectoryRequest(parentDirectory.DirectoryId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteDirectory_WhenValid_ShouldRemoveDirectoryFromDatabase()
        {
            // Arrange
            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, _user.Id)
                .Create();

            await SetupTestData(mockDirectory);

            // Act
            var response = await DeleteDirectoryRequest(mockDirectory.DirectoryId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(_user.Id, mockDirectory.DirectoryId);

            databaseResponse.Should().BeNull();
        }

        private async Task<HttpResponseMessage> DeleteDirectoryRequest(Guid id)
        {
            // setup request
            var uri = new Uri($"/document-service/api/directory/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Method = HttpMethod.Delete;
            message.Headers.Add(TokenService.Constants.AuthToken, _token);


            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
