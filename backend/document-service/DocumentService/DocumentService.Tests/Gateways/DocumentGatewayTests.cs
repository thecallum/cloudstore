using AutoFixture;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
{
    public class DocumentGatewayTests : BaseIntegrationTest
    {
        private readonly DocumentGateway _gateway;

        public DocumentGatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _gateway = new DocumentGateway(_context);
        }

        [Fact]
        public async Task SaveDocument_WhenCalled_InsertsDocumentIntoDatabase()
        {
            // Arrange
            var mockDocument = _fixture.Create<Document>();

            // Act 
            await _gateway.SaveDocument(mockDocument).ConfigureAwait(false);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentDb>(mockDocument.UserId, mockDocument.Id).ConfigureAwait(false);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(mockDocument.Name);
            databaseResponse.S3Location.Should().Be(mockDocument.S3Location);
            databaseResponse.FileSize.Should().Be(mockDocument.FileSize);

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(mockDocument.UserId, mockDocument.Id));
        }

        [Fact]
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act 
            var response = await _gateway.GetAllDocuments(userId);

            // Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDocumentsExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var numberOfDocuments = _random.Next(2, 5);

            var mockDocuments = _fixture
                .Build<DocumentDb>()
               .With(x => x.UserId, userId)
               .With(x => x.DirectoryId, userId)
               .CreateMany(numberOfDocuments);

            await SetupTestData(mockDocuments);

            // Act 
            var response = await _gateway.GetAllDocuments(userId);

            // Assert
            response.Should().HaveCount(numberOfDocuments);
        }

        [Fact]
        public async Task GetAllDocuments_WhenDirectoryIdNotNull_ReturnsDocumentsWithinDirectory()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var mockDirectory = _fixture.Build<DirectoryDb>()
                                    .With(x => x.UserId, userId)
                                    .Create();

            await SetupTestData(mockDirectory);

            // add one document to root
            var mockRootDocument = _fixture.Build<DocumentDb>()
                                        .With(x => x.UserId, userId)
                                        .With(x => x.DirectoryId, userId)
                                        .Create();

            await SetupTestData(mockRootDocument);

            // add two docuents to directory
            var mockDocumentsInDirectory = _fixture.Build<DocumentDb>()
                                       .With(x => x.UserId, userId)
                                       .With(x => x.DirectoryId, mockDirectory.DirectoryId)
                                       .CreateMany(2);

            await SetupTestData(mockDocumentsInDirectory);

            // Act
            var response = await _gateway.GetAllDocuments(userId, mockDirectory.DirectoryId);

            // Assert

            response.Should().HaveCount(2);
        }

        [Fact]
        public async Task DirectoryContainsDocuments_WhenFalse_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act
            var response = await _gateway.DirectoryContainsFiles(userId, directoryId);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task DirectoryContainsDocuments_WhenTrue_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            var document = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, userId)
                .Create();

            await SetupTestData(document);

            // Act
            var response = await _gateway.DirectoryContainsFiles(userId, directoryId);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task GetDocumentById_WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            // Act
            var response = await _gateway.GetDocumentById(userId, documentId);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetDocumentById_WhenDocumentExists_ReturnsDocument()
        {
            // Arrange
            var document = _fixture.Create<DocumentDb>();
            await SetupTestData(document);

            // Act
            var response = await _gateway.GetDocumentById(document.UserId, document.DocumentId);

            // Assert
            response.Should().BeOfType(typeof(DocumentDb));
            response.S3Location.Should().Be(document.S3Location);
            response.Name.Should().Be(document.Name);
        }

        [Fact]
        public async Task DeleteDocument_WhenDoesntExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            // Act
            Func<Task<DocumentDb>> func = async () => await _gateway.DeleteDocument(userId, documentId);

            // Assert

            await func.Should().ThrowAsync<DocumentNotFoundException>();
        }

        [Fact]
        public async Task DeleteDocument_WhenItExists_RemovesDocumentFromDatabase()
        {
            // Arrange
            var document = _fixture.Create<DocumentDb>();
            await SetupTestData(document);

            // Act
            var response = await _gateway.DeleteDocument(document.UserId, document.DocumentId);

            // Assert
            response.DocumentId.Should().Be(document.DocumentId);

            var databaseResponse = await _context.LoadAsync<DocumentDb>(document.UserId, document.DocumentId);
            databaseResponse.Should().BeNull();
        }
    }
}