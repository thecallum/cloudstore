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
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class ValidateUploadedDocumentUseCaseTests
    {
        private readonly ValidateUploadedDocumentUseCase _useCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;

        private readonly Fixture _fixture = new Fixture();

        public ValidateUploadedDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _useCase = new ValidateUploadedDocumentUseCase(
                _mockS3Gateway.Object,
                _mockDocumentGateway.Object);
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.StorageCapacity, long.MaxValue)
                .Create();

            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync((DocumentUploadResponse)null);

            // Act
            var response = await _useCase.Execute(documentId, request, user);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenExceedsStorageCapacity_ExceptionThrown()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.StorageCapacity, long.MaxValue)
                .Create();

            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            _mockDocumentGateway
               .Setup(x => x.CanUploadFile(It.IsAny<User>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(false);

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            Func<Task<DocumentResponse>> func = async () => await _useCase.Execute(documentId, request, user);

            // Assert
            await func.Should().ThrowAsync<ExceededUsageCapacityException>();
        }

        [Fact]
        public async Task WhenDocumentExists_CallsS3Gateway()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.StorageCapacity, long.MaxValue)
                .Create();

            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            _mockDocumentGateway
               .Setup(x => x.CanUploadFile(It.IsAny<User>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            var response = await _useCase.Execute(documentId, request, user);

            // Assert
            _mockS3Gateway.Verify(x => x.MoveDocumentToStoreDirectory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task WhenNewDocument_CreatesDocumentInDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();
            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.ValidateUploadedDocument(It.IsAny<string>()))
                .ReturnsAsync(documentUploadResponse);

            _mockDocumentGateway
               .Setup(x => x.CanUploadFile(It.IsAny<User>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            // Act
            var response = await _useCase.Execute(documentId, request, user);

            // Assert
            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<DocumentDomain>()), Times.Once);

            response.Should().NotBeNull();
            response.Id.Should().Be(documentId);
            response.DirectoryId.Should().Be((Guid)request.DirectoryId);
            response.FileSize.Should().Be(documentUploadResponse.FileSize);
            response.Name.Should().Be(request.FileName);
            response.S3Location.Should().Be($"{user.Id}/{documentId}");
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

            _mockDocumentGateway
               .Setup(x => x.CanUploadFile(It.IsAny<User>(), It.IsAny<long>(), It.IsAny<long?>()))
               .ReturnsAsync(true);

            var existingDocument = _fixture.Create<DocumentDomain>();

            var user = _fixture.Build<User>()
                .With(x => x.StorageCapacity, long.MaxValue)
                .With(x => x.Id, existingDocument.UserId)
                .Create();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(existingDocument);

            // Act
            var response = await _useCase.Execute(existingDocument.Id, request, user);

            // Assert
            _mockDocumentGateway.Verify(x => x.UpdateDocument(It.IsAny<DocumentDomain>()), Times.Once);

            response.Should().BeEquivalentTo(existingDocument.ToResponse());
        }
    }
}
