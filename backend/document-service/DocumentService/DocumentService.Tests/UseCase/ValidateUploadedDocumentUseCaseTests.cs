using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
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

        private readonly long _accountStorageCapacity = 200;

        public ValidateUploadedDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockStorageServiceGateway = new Mock<IStorageServiceGateway>();

            _useCase = new ValidateUploadedDocumentUseCase(
                _mockS3Gateway.Object, 
                _mockDocumentGateway.Object,
                _mockStorageServiceGateway.Object,
                _accountStorageCapacity);
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
        public async Task WhenExceedsStorageCapacity_ExceptionThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(false);

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            Func<Task<Document>> func = async () => await _useCase.Execute(userId, documentId, request);

            // Assert
            await func.Should().ThrowAsync<ExceededUsageCapacityException>();
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

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

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
        public async Task WhenNewDocument_CreatesDocumentInDatabase()
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

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

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

        [Fact]
        public async Task WhenExistingDocument_ReplacesDocumentInDatabase()
        {
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            var existingDocument = _fixture.Create<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(existingDocument);

            // Act
            var response = await _useCase.Execute(existingDocument.UserId, existingDocument.DocumentId, request);

            // Assert
            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<Document>()), Times.Once);

            response.Should().BeEquivalentTo(existingDocument.ToDomain(), config => {
                return config.Excluding(x => x.FileSize);
            });
        }


        [Fact]
        public async Task WhenNewDocument_AddsFileToStorageService()
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

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            var existingDocument = _fixture.Create<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((DocumentDb) null);

            // Act
            var response = await _useCase.Execute(userId, documentId, request);

            // Assert
            _mockStorageServiceGateway.Verify(x => x.AddFile(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once);
            _mockStorageServiceGateway.Verify(x => x.ReplaceFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task WhenExistingDocument_AddsFileToStorageService()
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

            _mockStorageServiceGateway
               .Setup(x => x.CanUploadFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            var existingDocument = _fixture.Create<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(existingDocument);

            // Act
            var response = await _useCase.Execute(userId, documentId, request);

            // Assert
            _mockStorageServiceGateway.Verify(x => x.AddFile(It.IsAny<Guid>(), It.IsAny<long>()), Times.Never);
            _mockStorageServiceGateway.Verify(x => x.ReplaceFile(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<long>()), Times.Once);
        }
    }
}
