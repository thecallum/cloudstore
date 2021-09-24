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
    public class LoginTests : IDisposable
    {

        private readonly Fixture _fixture = new Fixture();

        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;

        private readonly Random _random = new Random();

        private readonly HttpClient _httpClient;

        private readonly DatabaseFixture<Startup> _testFixture;

        private readonly PasswordHasher _passwordHasher;

        public LoginTests(DatabaseFixture<Startup> testFixture)
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
        public async Task Login_WhenRequestObjectIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = (LoginRequestObject)null;

            // Act
            var response = await LoginRequest(invalidRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Headers.Should().NotContainKey(HeaderConstants.AuthToken);
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
            response.Headers.Should().NotContainKey(HeaderConstants.AuthToken);
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

            await SetupTestData(mockUser);

            var mockRequest = new LoginRequestObject
            {
                Email = mockUser.Email,
                Password = "sdfsdfjskdfj"
            };

            // Act
            var response = await LoginRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Headers.Should().NotContainKey(HeaderConstants.AuthToken);
        }

        [Fact]
        public async Task Login_WhenPasswordIsValid_ReturnsOk()
        {
            // Arrange
            var password = _fixture.Create<string>();
            var hash = CreateHash(password);

            var mockUser = _fixture.Build<UserDb>()
                .With(x => x.Hash, hash)
                .With(x => x.Email, "email@email.com")
                .Create();

            await SetupTestData(mockUser);


            var mockRequest = new LoginRequestObject
            {
                Email = mockUser.Email,
                Password = password
            };

            // Act
            var response = await LoginRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Should().ContainKey(HeaderConstants.AuthToken);
        }

        private string CreateHash(string password)
        {
            return _passwordHasher.Hash(password);
        }

        private async Task<HttpResponseMessage> LoginRequest(LoginRequestObject request)
        {
            var uri = new Uri($"/api/auth/login", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }


    }
}
