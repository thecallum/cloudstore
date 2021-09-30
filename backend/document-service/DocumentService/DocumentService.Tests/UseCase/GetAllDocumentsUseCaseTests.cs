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
    public class GetAllDocumentsUseCaseTests
    {
        private readonly IGetAllDocumentsUseCase _getAllDocumentsUseCase;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public GetAllDocumentsUseCaseTests()
        {
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _getAllDocumentsUseCase = new GetAllDocumentsUseCase(_mockDocumentGateway.Object);
        }


        // no documents, return empty list

        // many documents - return list

        [Fact]
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var gatewayResponse = _fixture.CreateMany<DocumentDb>(0);

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _getAllDocumentsUseCase.Execute(userId);

            // Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenMultipleDocumentsExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var numberOfDocuments = _random.Next(2, 5);
            var gatewayResponse = _fixture.CreateMany<DocumentDb>(numberOfDocuments);

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _getAllDocumentsUseCase.Execute(userId);

            // Assert
            response.Should().HaveCount(numberOfDocuments);
        }
    }
}
