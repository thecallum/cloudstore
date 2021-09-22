using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using authservice.Domain;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using AutoFixture;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace authservice.Tests.Gateway
{
    [Collection("Database collection")]
    public class UserGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;

        private readonly UserGateway _gateway;
        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        public UserGatewayTests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _gateway = new UserGateway(_context);

            _testFixture = testFixture;
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
        public async Task DeleteUser_WhenUserDoesntExist_ThrowsException()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();

            // Act 
            Func<Task> func = async () => await _gateway.DeleteUser(mockEmail).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task DeleteUser_WhenUserExists_DeletesUserFromDatabase()
        {
            // Arrange
            var mockUser = _fixture.Create<UserDb>();
            await SetupTestData(mockUser);

            // Act 
            Func<Task> func = async () => await _gateway.DeleteUser(mockUser.Email).ConfigureAwait(false);

            // Assert
            await func.Should().NotThrowAsync<UserNotFoundException>();

            var databaseResult = await _context.LoadAsync<UserDb>(mockUser.Email).ConfigureAwait(false);
            databaseResult.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_WhenUserDoesntExist_ReturnsNull()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();

            // Act 
            var response = await _gateway.GetUserByEmailAddress(mockEmail).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var mockUser = _fixture.Create<UserDb>();
            await SetupTestData(mockUser);

            // Act 
            var response = await _gateway.GetUserByEmailAddress(mockUser.Email).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(mockUser.Id);
            response.Email.Should().Be(mockUser.Email);
            response.FirstName.Should().Be(mockUser.FirstName);
            response.LastName.Should().Be(mockUser.LastName);
        }

        [Fact]
        public async Task RegisterUser_WhenEmailAlreadyUsed_ThrowsException()
        {
            // Arrange
            var mockExistingUser = _fixture.Create<UserDb>();
            await SetupTestData(mockExistingUser);

            var mockNewUser = _fixture.Build<User>().With(x => x.Email, mockExistingUser.Email).Create();

            // Act 
            Func<Task> func = async () => await _gateway.RegisterUser(mockNewUser).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<UserWithEmailAlreadyExistsException>();
        }

        [Fact]
        public async Task RegisterUser_WhenNewEmail_CreatesUserInDatabase()
        {
            // Arrange
            var mockNewUser = _fixture.Create<User>();

            // Act 
            await _gateway.RegisterUser(mockNewUser).ConfigureAwait(false);

            // Assert
            var databaseResponse = await _context.LoadAsync<UserDb>(mockNewUser.Email).ConfigureAwait(false);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Id.Should().Be(mockNewUser.Id);
            databaseResponse.Email.Should().Be(mockNewUser.Email);
            databaseResponse.FirstName.Should().Be(mockNewUser.FirstName);
            databaseResponse.LastName.Should().Be(mockNewUser.LastName);
        }
    }
}