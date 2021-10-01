using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Domain;
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
        private readonly Mock<IGetAllDocumentsUseCase> _mockGetAllDocumentsUseCase;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DocumentControllerTests()
        {
            _mockUploadDocumentUseCase = new Mock<IUploadDocumentUseCase>();
            _mockGetAllDocumentsUseCase = new Mock<IGetAllDocumentsUseCase>();

            _documentController = new DocumentController(_mockUploadDocumentUseCase.Object, _mockGetAllDocumentsUseCase.Object);
        }

        [Fact]
        public async Task UploadDocument_WhenDirectoryNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                DirectoryId = Guid.NewGuid(),
                FilePath = ""
            };

            var exception = new DirectoryNotFoundException();

            _mockUploadDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<UploadDocumentRequest>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.UploadDocument(mockRequest);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(mockRequest.DirectoryId);
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

        [Fact]
        public async Task GetAllDocuments_WhenDirectoryDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = Guid.NewGuid() };

            var exception = new DirectoryNotFoundException();

            _mockGetAllDocumentsUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetAllDocumentsQuery>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.GetAllDocuments(query);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.DirectoryId);
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsNoDocuments()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            var useCaseResponse = new GetAllDocumentsResponse
            {
                Documents = new List<Document>()
            };

            _mockGetAllDocumentsUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetAllDocumentsQuery>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _documentController.GetAllDocuments(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(GetAllDocumentsResponse));
            ((response as OkObjectResult).Value as GetAllDocumentsResponse).Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDocumentsExist_ReturnsManyDocuments()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            var numberOfDocuments = _random.Next(2, 5);

            var useCaseResponse = new GetAllDocumentsResponse
            {
                Documents = _fixture.CreateMany<Document>(numberOfDocuments).ToList()
            };

            _mockGetAllDocumentsUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetAllDocumentsQuery>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _documentController.GetAllDocuments(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(GetAllDocumentsResponse));
            ((response as OkObjectResult).Value as GetAllDocumentsResponse).Documents.Should().HaveCount(numberOfDocuments);
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDirectoriesExist_ReturnsNoDirectories()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            var useCaseResponse = new GetAllDocumentsResponse
            {
                Directories = new List<Directory>()
            };

            _mockGetAllDocumentsUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetAllDocumentsQuery>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _documentController.GetAllDocuments(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(GetAllDocumentsResponse));
            ((response as OkObjectResult).Value as GetAllDocumentsResponse).Directories.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDirectoriesExist_ReturnsManyDirectories()
        {
            // Arrange
            var query = new GetAllDocumentsQuery { DirectoryId = null };

            var numberOfDirectories = _random.Next(2, 5);

            var useCaseResponse = new GetAllDocumentsResponse
            {
                Directories = _fixture.CreateMany<Directory>(numberOfDirectories).ToList()
            };

            _mockGetAllDocumentsUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetAllDocumentsQuery>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _documentController.GetAllDocuments(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(GetAllDocumentsResponse));
            ((response as OkObjectResult).Value as GetAllDocumentsResponse).Directories.Should().HaveCount(numberOfDirectories);
        }
    }
}
