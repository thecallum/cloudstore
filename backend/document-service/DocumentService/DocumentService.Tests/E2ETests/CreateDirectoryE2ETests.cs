using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Factories;
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

            var databaseResponse = await _dbFixture.GetDirectoryById(responseId); 

            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(_user.Id);
            databaseResponse.Name.Should().Be(request.Name);
            databaseResponse.ParentDirectoryId.Should().Be((Guid)request.ParentDirectoryId);
        }

        [Fact]
        public async Task CreateDirectory_WhenParentDirectoryExists_IncludesParentDirectoryIds()
        {
            // Arrange
            var mockParentDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.UserId, _user.Id)
                .Create()
                .ToDatabase(null);

            mockParentDirectory.ParentDirectoryId = Guid.NewGuid();
            mockParentDirectory.ParentDirectoryIds = $"{Guid.NewGuid()}/{mockParentDirectory.ParentDirectoryId}";

            await _dbFixture.SetupTestData(mockParentDirectory);

            var request = new CreateDirectoryRequest
            {
                Name = _fixture.Create<string>(),
                ParentDirectoryId = mockParentDirectory.Id
            };

            // Act
            var response = await CreateDirectoryRequest(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseId = Guid.Parse(response.Headers.Location.ToString());

            var databaseResponse = await _dbFixture.GetDirectoryById(responseId);

            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(_user.Id);
            databaseResponse.Name.Should().Be(request.Name);
            databaseResponse.ParentDirectoryId.Should().Be((Guid)request.ParentDirectoryId);

            var expectedParentDirectoryIds = $"{mockParentDirectory.ParentDirectoryIds}/{mockParentDirectory.Id}";
            databaseResponse.ParentDirectoryIds.Should().Be(expectedParentDirectoryIds);
        }

        private async Task<HttpResponseMessage> CreateDirectoryRequest(CreateDirectoryRequest request)
        {
            // setup request
            var uri = new Uri($"/api/directory/", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Method = HttpMethod.Post;
            message.Headers.Add("Authorization", _token);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
