using System.Threading.Tasks;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.UseCase;
using authservice.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace authservice.Tests.UseCase
{
    public class LoginUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ILoginUseCase _loginUseCase;
        private readonly Mock<IUserGateway> _mockUserGateway;

        public LoginUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _loginUseCase = new LoginUseCase(_mockUserGateway.Object);
        }

        [Fact]
        public async Task WhenUserDoesntExist_ReturnsNull()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();

            _mockUserGateway.Setup(x => x.GetUserByEmailAddress(It.IsAny<string>())).ReturnsAsync((UserDb) null);

            // Act
            var response = await _loginUseCase.Execute(mockEmail);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenUserExists_ReturnsUser()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();
            var mockUser = _fixture.Create<UserDb>();

            _mockUserGateway.Setup(x => x.GetUserByEmailAddress(It.IsAny<string>())).ReturnsAsync(mockUser);

            // Act
            var response = await _loginUseCase.Execute(mockEmail);

            // Assert
            response.Should().NotBeNull();


            response.Id.Should().Be(mockUser.Id);
            response.Email.Should().Be(mockUser.Email);
            response.FirstName.Should().Be(mockUser.FirstName);
            response.LastName.Should().Be(mockUser.LastName);
            response.Hash.Should().Be(mockUser.Hash);
        }
    }
}