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
    public class CreateDirectoryE2ETests : BaseIntegrationTest
    {
        public CreateDirectoryE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {

        }

        [Theory]
        [InlineData("")]
        [InlineData("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss")]
        public async Task CreateDirectory_WhenNameIsInvalid_ReturnsBadRequest(string name)
        {
            // Arrange
            var request = new CreateDirectoryRequest { Name = name };

            // Act
            var response = await CreateDirectoryRequest(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateDirectory_WhenValid_ShouldCreateDirectoryInDatabase()
        {
            // Arrange
            var request = new CreateDirectoryRequest
            {
                Name = _fixture.Create<string>(),
                ParentDirectoryId = Guid.NewGuid()
            };

            // Act
            var response = await CreateDirectoryRequest(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseId = Guid.Parse(response.Headers.Location.ToString());

            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(userId, responseId);
            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(userId);
            databaseResponse.Name.Should().Be(request.Name);
            databaseResponse.ParentDirectoryId.Should().Be((Guid) request.ParentDirectoryId);

            _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(userId, responseId));
        }

        private async Task<HttpResponseMessage> CreateDirectoryRequest(CreateDirectoryRequest request)
        {
            var uri = new Uri("/api/directory/", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}
