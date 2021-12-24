using System;
using System.Threading.Tasks;
using DocumentService.Gateways;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace DocumentService.TestsUseCase
{
    public class DeleteUserUseCaseTests
    {
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IUserGateway> _mockUserGateway;

        public DeleteUserUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _deleteUserUseCase = new DeleteUserUseCase(_mockUserGateway.Object);
        }

        [Fact]
        public async Task WhenUserWithEmailDoesntExist_ThrowsUserNotFoundException()
        {
            // Arrange
            var mockId = Guid.NewGuid();

            var exception = new UserNotFoundException(mockId.ToString());

            _mockUserGateway
                .Setup(x => x.DeleteUser(It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task> func = async () => { await _deleteUserUseCase.Execute(mockId).ConfigureAwait(false); };

            // Assert
            await func.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task WhenUserWithEmailExists_DoesntThrowException()
        {
            // Arrange
            var mockId = Guid.NewGuid();

            // Act
            Func<Task> func = async () => { await _deleteUserUseCase.Execute(mockId).ConfigureAwait(false); };

            // Assert
            await func.Should().NotThrowAsync<UserNotFoundException>();
        }
    }
}