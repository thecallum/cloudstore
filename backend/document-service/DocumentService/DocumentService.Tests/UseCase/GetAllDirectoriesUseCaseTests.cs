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
    public class GetAllDirectoriesUseCaseTests
    {
        private readonly IGetAllDirectoriesUseCase _getAllDirectoriesUseCase;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public GetAllDirectoriesUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _getAllDirectoriesUseCase = new GetAllDirectoriesUseCase(_mockDirectoryGateway.Object);
        }

        [Fact]
        public async Task WhenDirectoryDoesntExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var exception = new DirectoryNotFoundException();

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task<GetAllDirectoriesResponse>> func = async () => await _getAllDirectoriesUseCase.Execute(userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task WhenNoDirectoriesExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var directoryGatewayResponse = _fixture.CreateMany<DirectoryDb>(0);

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(directoryGatewayResponse);

            // Act
            var response = await _getAllDirectoriesUseCase.Execute(userId);

            // Assert
            response.Directories.Should().HaveCount(0);
        }

        [Fact]
        public async Task WhenMultipleDirectoriesExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var numberOfDirectories = _random.Next(2, 5);
            var directoryGatewayResponse = _fixture.CreateMany<DirectoryDb>(numberOfDirectories);

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(directoryGatewayResponse);

            // Act
            var response = await _getAllDirectoriesUseCase.Execute(userId);

            // Assert
            response.Directories.Should().HaveCount(numberOfDirectories);
        }
    }
}
