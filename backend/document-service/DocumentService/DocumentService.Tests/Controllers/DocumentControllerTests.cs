using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Controllers
{
    public class DocumentControllerTests
    {
        private readonly DocumentController _documentController;
        private readonly Mock<IUploadDocumentUseCase> _mockUploadDocumentUseCase;

        private readonly Fixture _fixture = new Fixture();
        public DocumentControllerTests()
        {
            _mockUploadDocumentUseCase = new Mock<IUploadDocumentUseCase>();

            _documentController = new DocumentController(_mockUploadDocumentUseCase.Object);
        }

        [Fact]
        public async Task UploadDocument_WhenInvalidFilePathException_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();

            var exception = new InvalidFilePathException();

            _mockUploadDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.UploadDocument(mockRequest);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task UploadDocument_WhenFileTooLargeException_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();

            var exception = new FileTooLargeException();

            _mockUploadDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.UploadDocument(mockRequest);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task UploadDocument_NoException_ReturnsUploadDocumentResponse()
        {
            // Arrange
            var mockRequest = _fixture.Create<UploadDocumentRequest>();

            var uploadDocumentResponse = _fixture.Create<UploadDocumentResponse>();

            _mockUploadDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync(uploadDocumentResponse);

            // Act
            var response = await _documentController.UploadDocument(mockRequest);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().BeOfType(typeof(UploadDocumentResponse));

            ((response as CreatedResult).Value as UploadDocumentResponse).Name.Should().Be(uploadDocumentResponse.Name);
            ((response as CreatedResult).Value as UploadDocumentResponse).S3Location.Should().Be(uploadDocumentResponse.S3Location);
            ((response as CreatedResult).Value as UploadDocumentResponse).DocumentId.Should().Be(uploadDocumentResponse.DocumentId);
        }
    }
}
