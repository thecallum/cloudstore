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
    public class GetAllDocumentsUseCaseTests
    {
        private readonly IGetAllDocumentsUseCase _getAllDocumentsUseCase;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public GetAllDocumentsUseCaseTests()
        {
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _getAllDocumentsUseCase = new GetAllDocumentsUseCase(_mockDocumentGateway.Object);
        }

        [Fact]
        public async Task WhenNoDocumentsExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = _fixture.Create<GetAllDocumentsQuery>();

            var documentGatewayResponse = _fixture.CreateMany<DocumentDb>(0);

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(documentGatewayResponse);

            var directoryGatewayResponse = _fixture.CreateMany<DirectoryDb>(0);

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>()))
                .ReturnsAsync(directoryGatewayResponse);

            // Act
            var response = await _getAllDocumentsUseCase.Execute(userId, query);

            // Assert
            response.Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task WhenMultipleDocumentsExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = _fixture.Create<GetAllDocumentsQuery>();

            var numberOfDocuments = _random.Next(2, 5);
            var documentGatewayResponse = _fixture.CreateMany<DocumentDb>(numberOfDocuments);

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(documentGatewayResponse);

            var directoryGatewayResponse = _fixture.CreateMany<DirectoryDb>(0);

            _mockDirectoryGateway
                .Setup(x => x.GetAllDirectories(It.IsAny<Guid>()))
                .ReturnsAsync(directoryGatewayResponse);

            // Act
            var response = await _getAllDocumentsUseCase.Execute(userId, query);

            // Assert
            response.Documents.Should().HaveCount(numberOfDocuments);
        }
    }
}
