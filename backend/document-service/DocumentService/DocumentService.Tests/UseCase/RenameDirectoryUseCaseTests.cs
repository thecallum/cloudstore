using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class RenameDirectoryUseCaseTests
    {
        private readonly RenameDirectoryUseCase _useCase;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly Fixture _fixture = new Fixture();
        public RenameDirectoryUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _useCase = new RenameDirectoryUseCase(_mockDirectoryGateway.Object);
        }

        [Fact]
        public async Task Rename_WhenDirectoryNotFound_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<RenameDirectoryQuery>();
            var request = _fixture.Create<RenameDirectoryRequest>();
            var userId = Guid.NewGuid();

            var exception = new DirectoryNotFoundException();

            _mockDirectoryGateway
                .Setup(x => x.RenameDirectory(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task> func = async () => await _useCase.Execute(query, request, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task Rename_WhenValid_CallsGateway()
        {
            // Arrange
            var query = _fixture.Create<RenameDirectoryQuery>();
            var request = _fixture.Create<RenameDirectoryRequest>();
            var userId = Guid.NewGuid();

            // Act
            await _useCase.Execute(query, request, userId);

            // Assert
            _mockDirectoryGateway.Verify(x => x.RenameDirectory(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>()));
        }
    }
}
