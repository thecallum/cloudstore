using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using authservice.Infrastructure;
using AutoFixture;
using FluentAssertions;
using TokenService.Models;
using Xunit;

namespace authservice.Tests.E2ETests
{
    [Collection("Database collection")]
    public class DeleteTests : IDisposable
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        private readonly TokenService.TokenService _tokenService;

        public DeleteTests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;

            _httpClient = testFixture.Client;

            _tokenService = new TokenService.TokenService(Environment.GetEnvironmentVariable("SECRET"));
        }

        public void Dispose()
        {
            _testFixture.ResetDatabase().GetAwaiter().GetResult();
        }

        private async Task SetupTestData(UserDb user)
        {
            await _context.SaveAsync(user).ConfigureAwait(false);
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

        [Fact]
        public async Task Delete_WhenUserDoesntExist_ReturnsBadRequest()
        {
            // Arrange
            var payload = _fixture.Create<Payload>();

            var token = CreateToken(payload);

            // Act
            var response = await DeleteRequest(token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenValid_DeletesUserFromDatabase()
        {
            // Arrange
            var mockUser = _fixture.Create<UserDb>();

            await SetupTestData(mockUser);

            var payload = new Payload
            {
                Email = mockUser.Email,
                FirstName = mockUser.FirstName,
                LastName = mockUser.LastName,
                Id = mockUser.Id
            };

            var token = CreateToken(payload);

            // Act
            var response = await DeleteRequest(token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var databaseResponse = await _context.LoadAsync<UserDb>(mockUser.Email).ConfigureAwait(false);
            databaseResponse.Should().BeNull();
        }

        private string CreateToken(Payload payload)
        {
            return _tokenService.CreateToken(payload);
        }

        private async Task<HttpResponseMessage> DeleteRequest(string token)
        {
            var uri = new Uri("/api/auth/delete", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Delete, uri);

            message.Headers.Add("Token", token);

            var response = await _httpClient.SendAsync(message).ConfigureAwait(false);

            return response;
        }
    }
}