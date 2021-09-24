using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using authservice.Boundary.Request;
using authservice.Encryption;
using authservice.Infrastructure;
using authservice.JWT;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace authservice.Tests.E2ETests
{
    [Collection("Database collection")]
    public class RegisterTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;

        private readonly Random _random = new Random();

        private readonly HttpClient _httpClient;

        private readonly DatabaseFixture<Startup> _testFixture;

        private readonly PasswordHasher _passwordHasher;

        public RegisterTests(DatabaseFixture<Startup> testFixture)
        {


            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;

            _httpClient = testFixture.Client;

            _passwordHasher = new PasswordHasher();
        }

        private async Task SetupTestData(UserDb user)
        {
            await _context.SaveAsync(user).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _testFixture.ResetDatabase().GetAwaiter().GetResult();
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
                            .With(x => x.Email, "email@email.com")
                            .Create();

            await SetupTestData(mockUser);

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
                Email = "email@email.com",
                Password = "aaaaaaaa",
                FirstName = "aaaaa",
                LastName = "aaaaa"
            };

            _fixture.Build<RegisterRequestObject>()
                        .With(x => x.Email, "email@email.com")
                        .With(x => x.Password, "aaaaaaaa")
                        .With(x => x.FirstName, "aaaaa")
                        .With(x => x.LastName, "aaaaa")
                        .Create();
            // Act
            var response = await RegisterRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var databaseResponse = await _context.LoadAsync<UserDb>(mockRequest.Email).ConfigureAwait(false);

            databaseResponse.FirstName.Should().Be(mockRequest.FirstName);
            databaseResponse.LastName.Should().Be(mockRequest.LastName);
            databaseResponse.Email.Should().Be(mockRequest.Email);
        }

        private async Task<HttpResponseMessage> RegisterRequest(RegisterRequestObject request)
        {
            var uri = new Uri($"/api/auth/register", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}
