using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class ValidateUploadedDocumentUseCaseTests
    {
        private readonly ValidateUploadedDocumentUseCase _useCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IStorageServiceGateway> _mockStorageServiceGateway;

        private readonly Fixture _fixture = new Fixture();

        public ValidateUploadedDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockStorageServiceGateway = new Mock<IStorageServiceGateway>();

            _useCase = new ValidateUploadedDocumentUseCase(
                _mockS3Gateway.Object, 
                _mockDocumentGateway.Object,
                _mockStorageServiceGateway.Object);
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync((DocumentUploadResponse)null);

            // Act
            var response = await _useCase.Execute(userId, documentId, request);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenDocumentExists_CallsS3Gateway()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            var response = await _useCase.Execute(userId, documentId, request);

            // Assert
            _mockS3Gateway.Verify(x => x.MoveDocumentToStoreDirectory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task WhenDocumentExists_CreatesDocumentInDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            var response = await _useCase.Execute(userId, documentId, request);

            // Assert

            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<Document>()), Times.Once);

            response.Should().NotBeNull();
            response.Id.Should().Be(documentId);
            response.UserId.Should().Be(userId);
            response.DirectoryId.Should().Be((Guid)request.DirectoryId);
            response.FileSize.Should().Be(documentUploadResponse.FileSize);
            response.Name.Should().Be(request.FileName);
            response.S3Location.Should().Be($"{userId}/{documentId}");
        }
    }
}
