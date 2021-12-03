using AutoFixture;
using DocumentServiceListener.Boundary.Request;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.UseCase;
using DocumentServiceListener.UseCase.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.UseCase
{
    public class DeleteDirectoryUseCaseTests
    {
        private readonly IDeleteDirectoryUseCase _useCase;

        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IStorageServiceGateway> _mockStorageServiceGateway;
        private readonly Mock<IS3Gateway> _mockS3Gateway;


        private readonly Fixture _fixture = new Fixture();

        public DeleteDirectoryUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockStorageServiceGateway = new Mock<IStorageServiceGateway>();
            _mockS3Gateway = new Mock<IS3Gateway>();

            _useCase = new DeleteDirectoryUseCase(
                _mockDirectoryGateway.Object,
                _mockDocumentGateway.Object,
                _mockStorageServiceGateway.Object,
                _mockS3Gateway.Object
            );
        }

        [Fact]
        public async Task WhenCalled_DeletesDirectoryFromDatabase()
        {
            // Arrange
            var message = _fixture.Create<SnsEvent>();

            var gatewayResponse = new List<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            await _useCase.ProcessMessageAsync(message);

            // Assert
            _mockDirectoryGateway.Verify(x => x.DeleteDirectory(message.TargetId, message.User.Id), Times.Once);
        }

        [Fact]
        public async Task WhenDirectoryContainsNoDocuments_DoesntCallGateways()
        {
            // Arrange
            var message = _fixture.Create<SnsEvent>();

            var gatewayResponse = new List<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            await _useCase.ProcessMessageAsync(message);

            // Assert
            _mockS3Gateway.Verify(x => x.DeleteDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Never);
            _mockDocumentGateway.Verify(x => x.DeleteDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Never);
            _mockStorageServiceGateway.Verify(x => x.RemoveDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Never);

        }

        [Fact]
        public async Task WhenDirectoryContainsDocuments_CallsGateways()
        {
            // Arrange
            var message = _fixture.Create<SnsEvent>();

            var gatewayResponse = _fixture.CreateMany<DocumentDb>(3).ToList();

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            await _useCase.ProcessMessageAsync(message);

            // Assert
            _mockS3Gateway.Verify(x => x.DeleteDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Once);
            _mockDocumentGateway.Verify(x => x.DeleteDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Once);
            _mockStorageServiceGateway.Verify(x => x.RemoveDocuments(It.IsAny<List<DocumentDb>>(), It.IsAny<Guid>()), Times.Once);

        }
    }
}
