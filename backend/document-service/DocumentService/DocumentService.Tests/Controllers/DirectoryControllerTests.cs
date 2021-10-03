using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Domain;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace DocumentService.Tests.Controllers
{
    public class DirectoryControllerTests
    {
        private readonly DirectoryController _directoryController;

        private readonly Mock<ICreateDirectoryUseCase> _mockCreateDirectoryUseCase;
        private readonly Mock<IRenameDirectoryUseCase> _mockRenameDirectoryUseCase;
        private readonly Mock<IDeleteDirectoryUseCase> _mockDeleteDirectoryUseCase;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DirectoryControllerTests()
        {
            _mockCreateDirectoryUseCase = new Mock<ICreateDirectoryUseCase>();
            _mockRenameDirectoryUseCase = new Mock<IRenameDirectoryUseCase>();
            _mockDeleteDirectoryUseCase = new Mock<IDeleteDirectoryUseCase>();

            _directoryController = new DirectoryController(_mockCreateDirectoryUseCase.Object, _mockRenameDirectoryUseCase.Object, _mockDeleteDirectoryUseCase.Object);
        }

        [Fact]
        public async Task Create_WhenCalled_CallsUseCase()
        {
            // Arrange
            var request = _fixture.Create<CreateDirectoryRequest>();

            // Act
            var response = await _directoryController.CreateDirectory(request);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));

            _mockCreateDirectoryUseCase.Verify(x => x.Execute(It.IsAny<CreateDirectoryRequest>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task Rename_WhenNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var query = _fixture.Create<RenameDirectoryQuery>();
            var request = _fixture.Create<RenameDirectoryRequest>();

            var exception = new DirectoryNotFoundException();

            _mockRenameDirectoryUseCase
                .Setup(x => x.Execute(It.IsAny<RenameDirectoryQuery>(), It.IsAny<RenameDirectoryRequest>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _directoryController.RenameDirectory(query, request);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);
        }

        [Fact]
        public async Task Rename_WhenValid_CallsUseCase()
        {
            // Arrange
            var query = _fixture.Create<RenameDirectoryQuery>();
            var request = _fixture.Create<RenameDirectoryRequest>();

            // Act
            var response = await _directoryController.RenameDirectory(query, request);

            // Assert
            response.Should().BeOfType(typeof(OkResult));

            _mockRenameDirectoryUseCase.Verify(x => x.Execute(It.IsAny<RenameDirectoryQuery>(), It.IsAny<RenameDirectoryRequest>(), It.IsAny<Guid>()));
        }

        [Fact]
        public async Task Delete_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();

            var exception = new DirectoryNotFoundException();

            _mockDeleteDirectoryUseCase
                .Setup(x => x.Execute(It.IsAny<DeleteDirectoryQuery>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _directoryController.DeleteDirectory(query);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.DirectoryId);
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsDocuments_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();

            var exception = new DirectoryContainsDocumentsException();

            _mockDeleteDirectoryUseCase
                .Setup(x => x.Execute(It.IsAny<DeleteDirectoryQuery>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _directoryController.DeleteDirectory(query);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task Delete_WhenDirectoryContainsChildDirectories_ThrowsException()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();

            var exception = new DirectoryContainsChildDirectoriesException();

            _mockDeleteDirectoryUseCase
                .Setup(x => x.Execute(It.IsAny<DeleteDirectoryQuery>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _directoryController.DeleteDirectory(query);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task Delete_WhenValid_CallsUseCase()
        {
            // Arrange
            var query = _fixture.Create<DeleteDirectoryQuery>();

            // Act
            var response = await _directoryController.DeleteDirectory(query);

            // Assert
            response.Should().BeOfType(typeof(OkResult));

            _mockDeleteDirectoryUseCase.Verify(x => x.Execute(It.IsAny<DeleteDirectoryQuery>(), It.IsAny<Guid>()));
        }
    }
}
