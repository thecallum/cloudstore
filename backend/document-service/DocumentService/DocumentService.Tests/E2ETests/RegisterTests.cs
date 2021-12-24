using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;
using DocumentService.Encryption;
using DocumentService.Infrastructure;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;
using DocumentService.Tests;

namespace DocumentService.Tests.E2ETests
{
    [Collection("Database collection")]
    public class RegisterTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly PasswordHasher _passwordHasher;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _dbFixture;

        public RegisterTests(DatabaseFixture<Startup> fixture)
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
        public async Task Register_WhenInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = (RegisterRequestObject)null;

            // Act
            var response = await RegisterRequest(invalidRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyUsed_ReturnsConflictResponse()
        {
            // Arrange
            var mockUser = _fixture.Build<UserDb>()
                .With(x => x.Email, "email3@email.com")
                .Create();

            await _dbFixture.SetupTestData(mockUser);

            var mockRequest = new RegisterRequestObject
            {
                Email = mockUser.Email,
                Password = "aaaaaaaa",
                FirstName = "aaaaa",
                LastName = "aaaaa"
            };

            // Act
            var response = await RegisterRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }      

        [Fact]
        public async Task Register_WhenValidRequest_CreatesUserInDatabase()
        {
            // Arrange
            var mockRequest = new RegisterRequestObject
            {
                Email = "email4@email.com",
                Password = "aaaaaaaa",
                FirstName = "aaaaa",
                LastName = "aaaaa"
            };

            // Act
            var response = await RegisterRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _dbFixture.GetUserByEmail(mockRequest.Email);

            databaseResponse.FirstName.Should().Be(mockRequest.FirstName);
            databaseResponse.LastName.Should().Be(mockRequest.LastName);
            databaseResponse.Email.Should().Be(mockRequest.Email);
        }

        private async Task<HttpResponseMessage> RegisterRequest(RegisterRequestObject request)
        {
            var uri = new Uri("/api/auth/register", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}