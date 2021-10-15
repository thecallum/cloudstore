using AutoFixture;
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
    public class DeleteDocumentUseCaseTests
    {
        private readonly IDeleteDocumentUseCase _deleteDocumentUseCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IStorageServiceGateway> _mockStorageServiceGateway;

        private readonly Fixture _fixture = new Fixture();

        public DeleteDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockStorageServiceGateway = new Mock<IStorageServiceGateway>();

            _deleteDocumentUseCase = new DeleteDocumentUseCase(
                _mockS3Gateway.Object, 
                _mockDocumentGateway.Object,
                _mockStorageServiceGateway.Object);
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentDoesntExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            var exception = new DocumentNotFoundException();

            _mockDocumentGateway
                .Setup(x => x.DeleteDocument(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task> func = async () => await _deleteDocumentUseCase.Execute(userId, documentId);

            // Assert
            await func.Should().ThrowAsync<DocumentNotFoundException>();

            _mockS3Gateway.Verify(x => x.DeleteDocument(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentExists_S3GatewayIsCalled()
        {
            // Arrange
            var document = _fixture.Create<DocumentDb>();

            var exception = new DocumentNotFoundException();

            _mockDocumentGateway
                .Setup(x => x.DeleteDocument(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(document);

            // Act
            await _deleteDocumentUseCase.Execute(document.UserId, document.DocumentId);

            // Assert
            _mockS3Gateway.Verify(x => x.DeleteDocument(document.S3Location), Times.Once);
        }
    }
}
