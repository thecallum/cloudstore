using AutoFixture;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
{
    [Collection("Database collection")]
    public class DocumentGatewayTests : IDisposable
    {
        private readonly DocumentGateway _gateway;
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DocumentGatewayTests()
        {
            _gateway = new DocumentGateway(InMemoryDb.Instance);
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        private async Task SetupTestData(DocumentDb document)
        {
            InMemoryDb.Instance.Documents.Add(document);

            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private async Task SetupTestData(IEnumerable<DocumentDb> documents)
        {
            foreach (var document in documents)
            {
                await SetupTestData(document);
            }
        }

        private async Task SetupTestData(DirectoryDb directory)
        {
            InMemoryDb.Instance.Directories.Add(directory);

            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private async Task SetupTestData(IEnumerable<DirectoryDb> directories)
        {
            foreach (var directory in directories)
            {
                await SetupTestData(directory);
            }
        }

        [Fact]
        public async Task SaveDocument_WhenCalled_InsertsDocumentIntoDatabase()
        {
            // Arrange
            var mockDocument = _fixture.Create<DocumentDomain>();

            // Act 
            await _gateway.SaveDocument(mockDocument).ConfigureAwait(false);

            // Assert
            var databaseResponse = await InMemoryDb.Instance.Documents.FindAsync(mockDocument.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(mockDocument.Name);
            databaseResponse.S3Location.Should().Be(mockDocument.S3Location);
            databaseResponse.FileSize.Should().Be(mockDocument.FileSize);
        }

        [Fact]
        public async Task UpdateDocument_WhenCalled_UpdatesExistingDocument()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var mockDocument = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, userId)
                .Create()
                .ToDatabase();

            await SetupTestData(mockDocument);

            var updatedDocument = new DocumentDomain
            {
                Id = mockDocument.Id,
                UserId = mockDocument.UserId,
                DirectoryId = mockDocument.DirectoryId,
                Name = mockDocument.Name,
                S3Location = mockDocument.S3Location
            };

            // currently, only filesize can be updated
            updatedDocument.FileSize = 1234;

            // Act 
            await _gateway.UpdateDocument(updatedDocument).ConfigureAwait(false);

            // Assert
            var databaseResponse = await InMemoryDb.Instance.Documents.FindAsync(mockDocument.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(mockDocument.Name);
            databaseResponse.S3Location.Should().Be(mockDocument.S3Location);
            databaseResponse.FileSize.Should().Be(updatedDocument.FileSize);
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
               .Build<DocumentDomain>()
               .With(x => x.UserId, userId)
               .With(x => x.DirectoryId, (Guid?) null)
               .CreateMany(numberOfDocuments)
               .Select(x => x.ToDatabase());

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

            var mockDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.UserId, userId)
                .Create()
                .ToDatabase();

            await SetupTestData(mockDirectory);

            // add one document to root
            var mockRootDocument = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, (Guid?) null)
                .Create()
                .ToDatabase();

            await SetupTestData(mockRootDocument);

            // add two docuents to directory
            var mockDocumentsInDirectory = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, mockDirectory.Id)
                .CreateMany(2)
                .Select(x => x.ToDatabase());

            await SetupTestData(mockDocumentsInDirectory);

            // Act
            var response = await _gateway.GetAllDocuments(userId, mockDirectory.Id);

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

            var document = _fixture.Build<DocumentDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, userId)
                .Create()
                .ToDatabase();

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
            var document = _fixture.Create<DocumentDomain>().ToDatabase();
            await SetupTestData(document);

            // Act
            var response = await _gateway.GetDocumentById(document.UserId, document.Id);

            // Assert
            response.Should().BeOfType(typeof(DocumentDomain));
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
            Func<Task<DocumentDomain>> func = async () => await _gateway.DeleteDocument(userId, documentId);

            // Assert

            await func.Should().ThrowAsync<DocumentNotFoundException>();
        }

        [Fact]
        public async Task DeleteDocument_WhenItExists_RemovesDocumentFromDatabase()
        {
            // Arrange
            var document = _fixture.Create<DocumentDomain>().ToDatabase();
            await SetupTestData(document);

            // Act
            var response = await _gateway.DeleteDocument(document.UserId, document.Id);

            // Assert
            response.Id.Should().Be(document.Id);

            var databaseResponse = await InMemoryDb.Instance.Documents.FindAsync(document.Id);
            databaseResponse.Should().BeNull();
        }
    }
}