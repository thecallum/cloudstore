using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Domain;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Tests.Helpers;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        private readonly Mock<IGetDocumentUploadLinkUseCase> _mockGetDocumentUploadLinkUseCase;
        private readonly Mock<IValidateUploadedDocumentUseCase> _mockValidateUploadedDocumentUseCase;
        private readonly Mock<IGetAllDocumentsUseCase> _mockGetAllDocumentsUseCase;
        private readonly Mock<IGetDocumentDownloadLinkUseCase> _mockGetDocumentDownloadLinkUseCase;
        private readonly Mock<IDeleteDocumentUseCase> _mockDeleteDocumentUseCase;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DocumentControllerTests()
        {
            _mockGetDocumentUploadLinkUseCase = new Mock<IGetDocumentUploadLinkUseCase>();
            _mockValidateUploadedDocumentUseCase = new Mock<IValidateUploadedDocumentUseCase>();
            _mockGetAllDocumentsUseCase = new Mock<IGetAllDocumentsUseCase>();
            _mockGetDocumentDownloadLinkUseCase = new Mock<IGetDocumentDownloadLinkUseCase>();
            _mockDeleteDocumentUseCase = new Mock<IDeleteDocumentUseCase>();

            _documentController = new DocumentController(
                _mockGetAllDocumentsUseCase.Object,
                _mockGetDocumentDownloadLinkUseCase.Object,
                _mockGetDocumentUploadLinkUseCase.Object,
                _mockValidateUploadedDocumentUseCase.Object, 
                _mockDeleteDocumentUseCase.Object);

            _documentController.ControllerContext = new ControllerContext();
            _documentController.ControllerContext.HttpContext = new DefaultHttpContext();
            _documentController.ControllerContext.HttpContext.Items["user"] = ContextHelper.CreateUser();
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
        public async Task GetDocumentDownloadLink_WhenDocumentNotFound_ReturnsNotFound()
        {
            // Arrange
            var query = _fixture.Create<GetDocumentLinkQuery>();

            var exception = new DocumentNotFoundException();

            _mockGetDocumentDownloadLinkUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.GetDocumentDownloadLink(query);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.DocumentId);
        }

        [Fact]
        public async Task GetDocumentDownloadLink_WhenValid_ReturnsGetDocumentResponse()
        {
            // Arrange
            var query = _fixture.Create<GetDocumentLinkQuery>();
            var useCaseResponse = _fixture.Create<string>();

            _mockGetDocumentDownloadLinkUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _documentController.GetDocumentDownloadLink(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            ((response as OkObjectResult).Value as GetDocumentLinkResponse).DocumentLink.Should().Be(useCaseResponse);
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<DeleteDocumentRequest>();

            var exception = new DocumentNotFoundException();

            _mockDeleteDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _documentController.DeleteDocument(request);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(request.DocumentId);
        }

        [Fact]
        public async Task DeleteDocument_WhenDocumentFound_ReturnsNoContentResponse()
        {
            // Arrange
            var request = _fixture.Create<DeleteDocumentRequest>();

            // Act
            var response = await _documentController.DeleteDocument(request);

            // Assert
            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public void GetDocumentUploadLink_WhenCalled_ReturnsLink()
        {
            // Arrange
            var uploadResponseObject = new GetDocumentUploadResponse
            {
                UploadUrl = _fixture.Create<string>(),
                DocumentId = Guid.NewGuid()
            };

            var query = new GetDocumentUploadLinkQuery { ExistingDocumentId = Guid.NewGuid() };

            _mockGetDocumentUploadLinkUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<GetDocumentUploadLinkQuery>()))
                .Returns(uploadResponseObject);

            // Act
            var response = _documentController.GetDocumentUploadLink(query);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));

            ((response as OkObjectResult).Value as GetDocumentUploadResponse).UploadUrl.Should().Be(uploadResponseObject.UploadUrl);
            ((response as OkObjectResult).Value as GetDocumentUploadResponse).DocumentId.Should().Be(uploadResponseObject.DocumentId);
        }

        [Fact]
        public async Task ValidateUploadedDocument_WhenDocumentNotDoesntExist_Returns404NotFound()
        {
            // Arrange
            var query = _fixture.Create<ValidateUploadedDocumentQuery>();
            var request = _fixture.Create<ValidateUploadedDocumentRequest>();

            _mockValidateUploadedDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<ValidateUploadedDocumentRequest>()))
                .ReturnsAsync((Document) null);

            // Act
            var response = await _documentController.ValidateUploadedDocument(query, request);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.DocumentId);
        }

        [Fact]
        public async Task ValidateUploadedDocument_WhenDocumentExists_ReturnsCreatedResponse()
        {
            // Arrange
            var query = _fixture.Create<ValidateUploadedDocumentQuery>();
            var request = _fixture.Create<ValidateUploadedDocumentRequest>();

            var document = _fixture.Create<Document>();

            _mockValidateUploadedDocumentUseCase
                .Setup(x => x.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<ValidateUploadedDocumentRequest>()))
                .ReturnsAsync(document);

            // Act
            var response = await _documentController.ValidateUploadedDocument(query, request);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().Be(document);
        }
    }
}
