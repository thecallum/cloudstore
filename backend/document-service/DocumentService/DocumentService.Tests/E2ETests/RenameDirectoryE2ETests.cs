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
    public class RenameDirectoryE2ETests : BaseIntegrationTest
    {
        public RenameDirectoryE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {

        }

        // invalid name
        // directory doesnt exist
        // valid

        [Fact]
        public async Task RenameDirectory_WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<RenameDirectoryRequest>();
            var directoryId = Guid.NewGuid();

            // Act
            var response = await RenameDirectoryRequest(directoryId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss")]
        public async Task RenameDirectory(string name)
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .Create();

            await SetupTestData(mockDirectory);

            var request = new RenameDirectoryRequest { Name = name };

            // Act
            var response = await RenameDirectoryRequest(mockDirectory.DirectoryId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RenameDirectory_WhenValid_ShouldCreateDirectoryInDatabase()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, userId)
                .Create();

            await SetupTestData(mockDirectory);

            var request = new RenameDirectoryRequest { Name = "sssss" };

            var directoryId = mockDirectory.DirectoryId;

            // Act
            var response = await RenameDirectoryRequest(directoryId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(userId, mockDirectory.DirectoryId);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(request.Name);
        }

        private async Task<HttpResponseMessage> RenameDirectoryRequest(Guid id, RenameDirectoryRequest request)
        {
            var uri = new Uri($"/api/directory/{id}", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}

