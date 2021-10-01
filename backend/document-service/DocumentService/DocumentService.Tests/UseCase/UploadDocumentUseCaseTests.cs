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
    public class UploadDocumentUseCaseTests
    {
        private readonly IUploadDocumentUseCase _uploadDocumentUseCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly Fixture _fixture = new Fixture();
        public UploadDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _uploadDocumentUseCase = new UploadDocumentUseCase(_mockS3Gateway.Object, _mockDocumentGateway.Object, _mockDirectoryGateway.Object);
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryNotFound_ThrowsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

            _mockDirectoryGateway
                .Setup(x => x.CheckDirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            Func<Task<UploadDocumentResponse>> func = async () => await _uploadDocumentUseCase.Execute(mockRequest, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryExists_NoExceptionThrown()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

            _mockDirectoryGateway
                .Setup(x => x.CheckDirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            Func<Task<UploadDocumentResponse>> func = async () => await _uploadDocumentUseCase.Execute(mockRequest, userId);

            // Assert
            await func.Should().NotThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task UploadDocument_WhenFilePathInvalid_ThrowsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

            _mockDirectoryGateway
                .Setup(x => x.CheckDirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var exception = new InvalidFilePathException();

            _mockS3Gateway
                .Setup(x => x.UploadDocument(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task<UploadDocumentResponse>> func = async () => await _uploadDocumentUseCase.Execute(mockRequest, userId);

            // Assert
            await func.Should().ThrowAsync<InvalidFilePathException>();

            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<Document>()), Times.Never);
        }

        [Fact]
        public async Task UploadDocument_WhenFileTooLarge_ThrowsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

            _mockDirectoryGateway
                .Setup(x => x.CheckDirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var exception = new FileTooLargeException();

            _mockS3Gateway
                .Setup(x => x.UploadDocument(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task<UploadDocumentResponse>> func = async () => await _uploadDocumentUseCase.Execute(mockRequest, userId);

            // Assert
            await func.Should().ThrowAsync<FileTooLargeException>();

            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<Document>()), Times.Never);
        }

        [Fact]
        public async Task UploadDocument_WhenValid_ReturnsUploadDocumentResponse()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

            _mockDirectoryGateway
                .Setup(x => x.CheckDirectoryExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var documentUploadResponse = _fixture.Create<DocumentUploadResponse>();

            _mockS3Gateway
                .Setup(x => x.UploadDocument(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(documentUploadResponse);

            // Act
            var response = await _uploadDocumentUseCase.Execute(mockRequest, userId);

            // Assert
            response.Should().BeOfType(typeof(UploadDocumentResponse));
            response.Name.Should().Be(documentUploadResponse.DocumentName);
            response.S3Location.Should().Be(documentUploadResponse.S3Location);

            _mockDocumentGateway.Verify(x => x.SaveDocument(It.IsAny<Document>()), Times.Once);
        }

    }
}
