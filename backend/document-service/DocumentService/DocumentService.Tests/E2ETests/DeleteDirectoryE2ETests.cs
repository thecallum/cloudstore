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
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");
            var directoryId = Guid.NewGuid();

            var document = _fixture.Build<DocumentDb>()
                            .With(x => x.UserId, userId)
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
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var parentDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, userId)
                .Create();

            await SetupTestData(parentDirectory);

            var childDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, userId)
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
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .Create();

            await SetupTestData(mockDirectory);

            // Act
            var response = await DeleteDirectoryRequest(mockDirectory.DirectoryId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(userId, mockDirectory.DirectoryId);

            databaseResponse.Should().BeNull();
        }

        private async Task<HttpResponseMessage> DeleteDirectoryRequest(Guid id)
        {
            var uri = new Uri($"/document-service/api/directory/{id}", UriKind.Relative);

            var response = await _httpClient.DeleteAsync(uri).ConfigureAwait(false);

            return response;
        }
    }
}
