using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
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
    public class DeleteDirectoryUseCaseTests
    {
        private readonly DeleteDirectoryUseCase _useCase;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<ISnsGateway> _mockSnsGateway;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DeleteDirectoryUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockSnsGateway = new Mock<ISnsGateway>();

            _useCase = new DeleteDirectoryUseCase(
                _mockDirectoryGateway.Object, 
                _mockDocumentGateway.Object,
                _mockSnsGateway.Object);
        }

        [Fact]
        public async Task DeleteDirectory_WhenDirectoryNotFound_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var user = _fixture.Create<User>();

            _mockDirectoryGateway
                .Setup(x => x.DirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> func = async () => await _useCase.Execute(query, user);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task DeleteDirectory_WhenDirectoryExists_PublishesSnsEvents()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();
            var user = _fixture.Create<User>();

            _mockDirectoryGateway
                .Setup(x => x.DirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var numberOfDirectories = _random.Next(2, 5);

            var childDirectories = _fixture
               .CreateMany<DirectoryDb>(numberOfDirectories)
               .ToList();

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(childDirectories);

            // Act
            await _useCase.Execute(query, user);

            // Assert
            _mockSnsGateway
                .Verify(x => x.PublishDeleteDirectoryEvent(It.IsAny<User>(), It.IsAny<Guid>()), Times.Exactly(numberOfDirectories + 1));
        }
    }
}
