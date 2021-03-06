using System;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace DocumentService.TestsUseCase
{
    public class RegisterUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IUserGateway> _mockUserGateway;

        private readonly IRegisterUseCase _registerUseCase;

        public RegisterUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _registerUseCase = new RegisterUseCase(_mockUserGateway.Object);
        }

        [Fact]
        public async Task WhenCalled_ReturnsGuid()
        {
            // Arrange
            var mockRequest = _fixture.Create<RegisterRequestObject>();
            var mockUser = _fixture.Create<UserDb>();
            var mockHash = _fixture.Create<string>();

            _mockUserGateway.Setup(x => x.RegisterUser(It.IsAny<UserDb>()));

            // Act
            Func<Task<Guid>> func = async () =>
                await _registerUseCase.Execute(mockRequest, mockHash).ConfigureAwait(false);

            // Assert
            await func.Should().NotThrowAsync<UserWithEmailAlreadyExistsException>();
        }

        [Fact]
        public async Task WhenEmailAlreadyInUse_ThrowsUserWithEmailAlreadyExistsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<RegisterRequestObject>();
            var mockHash = _fixture.Create<string>();

            var exception = new UserWithEmailAlreadyExistsException(mockRequest.Email);

            _mockUserGateway
                .Setup(x => x.RegisterUser(It.IsAny<UserDb>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task<Guid>> func = async () =>
            {
                return await _registerUseCase.Execute(mockRequest, mockHash).ConfigureAwait(false);
            };

            // Assert
            await func.Should().ThrowAsync<UserWithEmailAlreadyExistsException>();
        }
    }
}