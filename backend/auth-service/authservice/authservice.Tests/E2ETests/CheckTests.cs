﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using authservice.Infrastructure;
using authservice.JWT;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace authservice.Tests.E2ETests
{
    [Collection("Database collection")]
    public class CheckTests : IDisposable
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        private readonly TokenService _tokenService;

        public CheckTests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;

            _httpClient = testFixture.Client;

            _tokenService = new TokenService(Environment.GetEnvironmentVariable("SECRET"));
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
        public async Task Check_WhenTokenInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var invalidToken = "jasd";

            // Act
            var response = await CheckRequest(invalidToken);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Check_WhenValidToken_ReturnsOk()
        {
            // Arrange
            var payload = _fixture.Create<Payload>();

            var token = CreateToken(payload);

            // Act
            var response = await CheckRequest(token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private string CreateToken(Payload payload)
        {
            return _tokenService.CreateToken(payload);
        }

        private async Task<HttpResponseMessage> CheckRequest(string token)
        {
            var uri = new Uri("/api/auth/check", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            message.Headers.Add("Token", token);

            var response = await _httpClient.SendAsync(message).ConfigureAwait(false);

            return response;
        }
    }
}