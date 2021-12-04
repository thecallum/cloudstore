using System;
using System.Threading.Tasks;
using authservice.Gateways;
using authservice.Infrastructure.Exceptions;
using authservice.UseCase;
using authservice.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace authservice.Tests.UseCase
{
    public class DeleteUseCaseTests
    {
        private readonly IDeleteUseCase _deleteUseCase;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IUserGateway> _mockUserGateway;

        public DeleteUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _deleteUseCase = new DeleteUseCase(_mockUserGateway.Object);
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
            Func<Task> func = async () => { await _deleteUseCase.Execute(mockId).ConfigureAwait(false); };

            // Assert
            await func.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task WhenUserWithEmailExists_DoesntThrowException()
        {
            // Arrange
            var mockId = Guid.NewGuid();

            // Act
            Func<Task> func = async () => { await _deleteUseCase.Execute(mockId).ConfigureAwait(false); };

            // Assert
            await func.Should().NotThrowAsync<UserNotFoundException>();
        }
    }
}