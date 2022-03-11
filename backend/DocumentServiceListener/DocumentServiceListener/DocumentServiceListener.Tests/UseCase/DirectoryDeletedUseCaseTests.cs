using AutoFixture;
using AWSServerless1;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Gateways.Interfaces;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.UseCase
{
    public class DirectoryDeletedUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly DirectoryDeletedUseCase _classUnderTest;

        public DirectoryDeletedUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _classUnderTest = new DirectoryDeletedUseCase(
                _mockS3Gateway.Object,
                _mockDocumentGateway.Object,
                _mockDirectoryGateway.Object);
        }

        [Fact]
        public async Task WhenMissingBodyParameters_ThrowsArgumentNullExceptionAsync()
        {
            // Arrange
            var entity = _fixture.Create<CloudStoreSnsEvent>();

            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task WhenNoDocumentFound_DoesntCallAnyUseCase()
        {
            // Arrange
            var directoryId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DirectoryId", directoryId.ToString());

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<DocumentDb>());

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            _mockS3Gateway
                .Verify(x => x.DeleteDocuments(It.IsAny<List<string>>()), Times.Never);

            _mockDocumentGateway
                .Verify(x => x.DeleteAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task WhenNoDocumentsFound_DeletesDirectoryAsync()
        {
            // Arrange
            var directoryId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DirectoryId", directoryId.ToString());

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<DocumentDb>());

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            _mockDirectoryGateway
                .Verify(x => x.DeleteDirectory(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task WhenDocumentsExist_DeletesRelatedData()
        {
            // Arrange
            var directoryId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DirectoryId", directoryId.ToString());

            var documents = _fixture.CreateMany<DocumentDb>(3);

            _mockDocumentGateway
                .Setup(x => x.GetAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(documents);

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            _mockS3Gateway
                .Verify(x => x.DeleteDocuments(It.IsAny<List<string>>()), Times.Exactly(2));

            _mockDocumentGateway            
                .Verify(x => x.DeleteAllDocuments(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);

            _mockDirectoryGateway
                .Verify(x => x.DeleteDirectory(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}
