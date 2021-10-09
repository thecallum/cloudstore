﻿using AutoFixture;
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
    public class RenameDirectoryE2ETests : BaseIntegrationTest
    {
        private readonly Guid _userId;
        private readonly string _token;

        public RenameDirectoryE2ETests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
            _userId = Guid.NewGuid();
            _token = ContextHelper.CreateToken(_userId);
        }

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
            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, _userId)
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
            var mockDirectory = _fixture.Build<DirectoryDb>()
                .With(x => x.UserId, _userId)
                .Create();

            await SetupTestData(mockDirectory);

            var request = new RenameDirectoryRequest { Name = "sssss" };

            var directoryId = mockDirectory.DirectoryId;

            // Act
            var response = await RenameDirectoryRequest(directoryId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(_userId, mockDirectory.DirectoryId);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(request.Name);
        }

        private async Task<HttpResponseMessage> RenameDirectoryRequest(Guid id, RenameDirectoryRequest request)
        {
            // setup request
            var uri = new Uri($"/document-service/api/directory/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Method = HttpMethod.Patch;
            message.Headers.Add("authorizationToken", _token);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
