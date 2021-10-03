using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
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
    public class DeleteDirectoryUseCaseTests
    {
        private readonly DeleteDirectoryUseCase _useCase;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;

        private readonly Fixture _fixture = new Fixture();
        public DeleteDirectoryUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _useCase = new DeleteDirectoryUseCase(_mockDirectoryGateway.Object, _mockDocumentGateway.Object);
        }

        [Fact]
        public async Task Delete_WhenDirectoryNotFound_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var userId = Guid.NewGuid();

            var exception = new DirectoryNotFoundException();

            _mockDirectoryGateway
                .Setup(x => x.DeleteDirectory(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task> func = async () => await _useCase.Execute(query, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsDocuments_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var userId = Guid.NewGuid();

            _mockDocumentGateway
                .Setup(x => x.DirectoryContainsFiles(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> func = async () => await _useCase.Execute(query, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryContainsDocumentsException>();
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsChildDirectories_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var userId = Guid.NewGuid();

            _mockDocumentGateway
                .Setup(x => x.DirectoryContainsFiles(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _mockDirectoryGateway
                .Setup(x => x.ContainsChildDirectories(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> func = async () => await _useCase.Execute(query, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryContainsChildDirectoriesException>();
        }

        [Fact]
        public async Task Delete_WhenValid_CallsGateway()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var userId = Guid.NewGuid();

            // Act
            await _useCase.Execute(query, userId);

            // Assert
            _mockDirectoryGateway.Verify(x => x.DeleteDirectory(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }
    }
}
