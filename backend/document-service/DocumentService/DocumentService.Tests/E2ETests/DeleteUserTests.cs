using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DocumentService.Infrastructure;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using DocumentService.Tests;
using Microsoft.EntityFrameworkCore;
using DocumentService.Domain;
using DocumentService.Services;

namespace DocumentService.Tests.E2ETests
{
    [Collection("Database collection")]
    public class DeleteUserTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly TokenService _tokenService;

        private readonly DatabaseFixture<Startup> _dbFixture;

        public DeleteUserTests(DatabaseFixture<Startup> fixture)
        {
            _dbFixture = fixture;
            _httpClient = _dbFixture.Client;

            _tokenService = new TokenService(Environment.GetEnvironmentVariable("SECRET"));
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        [Fact]
        public async Task Delete_WhenTokenIsInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var invalidToken = "osajhdokjashdjkloashd";

            // Act
            var response = await DeleteRequest(invalidToken);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //[Fact]
        //public async Task Delete_WhenUserDoesntExist_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var payload = _fixture.Create<User>();

        //    var token = _tokenService.CreateToken(payload);

        //    // Act
        //    var response = await DeleteRequest(token);

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //}

        [Fact]
        public async Task Delete_WhenValid_DeletesUserFromDatabase()
        {
            // Arrange
            var mockUser = _fixture.Create<UserDb>();

            await _dbFixture.SetupTestData(mockUser);

            var payload = new User
            {
                Email = mockUser.Email,
                FirstName = mockUser.FirstName,
                LastName = mockUser.LastName,
                Id = mockUser.Id
            };

            var token = _tokenService.CreateToken(payload);

            // Act
            var response = await DeleteRequest(token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var databaseResponse = await _dbFixture.GetUserById(mockUser.Id);
            databaseResponse.Should().BeNull();
        }

        private async Task<HttpResponseMessage> DeleteRequest(string token)
        {
            var uri = new Uri("/api/auth/delete", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Headers.Add("Authorization", token);

            var response = await _httpClient.SendAsync(message).ConfigureAwait(false);

            return response;
        }
    }
}