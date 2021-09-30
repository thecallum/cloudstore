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

        private readonly Fixture _fixture = new Fixture();
        public UploadDocumentUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _uploadDocumentUseCase = new UploadDocumentUseCase(_mockS3Gateway.Object, _mockDocumentGateway.Object);
        }

        [Fact]
        public async Task UploadDocument_WhenFilePathInvalid_ThrowsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();
            var userId = Guid.NewGuid();

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
