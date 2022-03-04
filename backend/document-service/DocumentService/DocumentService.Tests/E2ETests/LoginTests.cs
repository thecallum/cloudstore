using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;
using DocumentService.Encryption;
using DocumentService.Infrastructure;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using DocumentService.Tests;
using DocumentService.Services;

namespace DocumentService.Tests.E2ETests
{
    [Collection("Database collection")]
    public class LoginTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly PasswordHasher _passwordHasher;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _dbFixture;

        public LoginTests(DatabaseFixture<Startup> fixture)
        {

            _dbFixture = fixture;

            _httpClient = _dbFixture.Client;

            _passwordHasher = new PasswordHasher();
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        [Fact]
        public async Task Login_WhenRequestObjectIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = (LoginRequestObject)null;

            // Act
            var response = await LoginRequest(invalidRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Headers.Should().NotContainKey("Authorization");
        }

        [Fact]
        public async Task Login_WhenUserDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var mockRequest = new LoginRequestObject
            {
                Email = "email@email.com",
                Password = "sdfsdfjskdfj"
            };

            // Act
            var response = await LoginRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Headers.Should().NotContainKey("Authorization");
        }

        [Fact]
        public async Task Login_WhenPasswordIsInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var randomHash = CreateHash(_fixture.Create<string>());

            var mockUser = _fixture.Build<UserDb>()
                .With(x => x.Email, "email@email.com")
                .With(x => x.Hash, randomHash)
                .Create();

            await _dbFixture.SetupTestData(mockUser);

            var mockRequest = new LoginRequestObject
            {
                Email = mockUser.Email,
                Password = "sdfsdfjskdfj"
            };

            // Act
            var response = await LoginRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Headers.Should().NotContainKey("Authorization");
        }

        [Fact]
        public async Task Login_WhenPasswordIsValid_ReturnsOk()
        {
            // Arrange
            var password = _fixture.Create<string>();
            var hash = CreateHash(password);

            var mockUser = _fixture.Build<UserDb>()
                .With(x => x.Hash, hash)
                .With(x => x.Email, "email5@email.com")
                .Create();

            await _dbFixture.SetupTestData(mockUser);

            var mockRequest = new LoginRequestObject
            {
                Email = mockUser.Email,
                Password = password
            };

            // Act
            var response = await LoginRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Should().ContainKey("Authorization");
        }

        private string CreateHash(string password)
        {
            return _passwordHasher.Hash(password);
        }

        private async Task<HttpResponseMessage> LoginRequest(LoginRequestObject request)
        {
            var uri = new Uri("/api/auth/login", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}