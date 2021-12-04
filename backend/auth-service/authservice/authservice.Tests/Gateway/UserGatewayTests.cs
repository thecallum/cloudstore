using System;
using System.Threading.Tasks;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace authservice.Tests.Gateway
{
    [Collection("Database collection")]
    public class UserGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly UserGateway _gateway;
        private readonly Random _random = new Random();

        public UserGatewayTests()
        {
            _gateway = new UserGateway(InMemoryDb.Instance);
        }


        public void Dispose()
        {
            InMemoryDb.Teardown();

            //_testFixture.ResetDatabase().GetAwaiter().GetResult();
        }


        private async Task SetupTestData(User user)
        {
            InMemoryDb.Instance.Users.Add(user);

            await InMemoryDb.Instance.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteUser_WhenUserDoesntExist_ThrowsException()
        {
            // Arrange
            var mockId = Guid.NewGuid();

            // Act 
            Func<Task> func = async () => await _gateway.DeleteUser(mockId).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task DeleteUser_WhenUserExists_DeletesUserFromDatabase()
        {
            // Arrange
            var mockUser = _fixture.Create<User>();
            await SetupTestData(mockUser);

            // Act 
            Func<Task> func = async () => await _gateway.DeleteUser(mockUser.Id).ConfigureAwait(false);

            // Assert
            await func.Should().NotThrowAsync<UserNotFoundException>();

            var databaseResult = await InMemoryDb.Instance.Users.FindAsync(mockUser.Id);
            databaseResult.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_WhenUserDoesntExist_ReturnsNull()
        {
            // Arrange
            var mockId = Guid.NewGuid();

            // Act 
            var response = await _gateway.GetUserById(mockId).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var mockUser = _fixture.Create<User>();
            await SetupTestData(mockUser);

            // Act 
            var response = await _gateway.GetUserById(mockUser.Id).ConfigureAwait(false);

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
            var mockExistingUser = _fixture.Create<User>();
            mockExistingUser.Email = "email20@gmail.com";

            await SetupTestData(mockExistingUser);

            var mockNewUser = _fixture.Build<User>().With(x => x.Email, mockExistingUser.Email).Create();

            // Act 
            Func<Task> func = async () => await _gateway.RegisterUser(mockNewUser).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<UserWithEmailAlreadyExistsException>();
        }

        [Fact]
        public async Task RegisterUser_WhenEmailAlreadyUsedWithDifferentCase_ThrowsException()
        {
            // Arrange
            var mockExistingUser = _fixture.Create<User>();
            mockExistingUser.Email = "email40@gmail.com";

            var mockNewUserEmail = "EMAIL40@GMAIL.COM";

            await SetupTestData(mockExistingUser);

            var mockNewUser = _fixture.Build<User>().With(x => x.Email, mockNewUserEmail).Create();

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
            var databaseResponse = await InMemoryDb.Instance.Users.FindAsync(mockNewUser.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Id.Should().Be(mockNewUser.Id);
            databaseResponse.Email.Should().Be(mockNewUser.Email);
            databaseResponse.FirstName.Should().Be(mockNewUser.FirstName);
            databaseResponse.LastName.Should().Be(mockNewUser.LastName);
        }

        [Fact]
        public async Task GetUserByEmail_WhenEntityDoesntExist_ReturnsNull()
        {
            // Arrange
            var mockEmail = "email@email.com";

            // Act 
            var response = await _gateway.GetUserByEmail(mockEmail);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmail_WhenEntityExists_ReturnsUser()
        {
            // Arrange
            var mockExistingUser = _fixture.Create<User>();
            await SetupTestData(mockExistingUser);

            // Act 
            var response = await _gateway.GetUserByEmail(mockExistingUser.Email);

            // Assert
            response.Should().Be(mockExistingUser);
        }

        [Fact]
        public async Task GetUserByEmail_WhenEntityExistsWithEmailInDifferentCase_ReturnsUser()
        {
            // Arrange
            var mockExistingUser = _fixture.Create<User>();
            mockExistingUser.Email = "email100@email.com";
            await SetupTestData(mockExistingUser);

            var differentCaseEmail = "EMAIL100@EMAIL.COM";

            // Act 
            var response = await _gateway.GetUserByEmail(differentCaseEmail);

            // Assert
            response.Should().Be(mockExistingUser);
        }
    }
}