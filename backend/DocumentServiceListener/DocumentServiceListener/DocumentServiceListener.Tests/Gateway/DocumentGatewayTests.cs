using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateway
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

        [Fact]
        public async Task UpdateThumbnail_WhenCalled_UpdatesImageThumbnail()
        {
            // Arrange
            var document = _fixture.Create<DocumentDb>();
            await SetupTestData(document);

            // Act
            await _gateway.UpdateThumbnail(document.UserId, document.Id);

            // Assert
            var expectedThumbnailUrl = $"https://uploadfromcs.s3.eu-west-1.amazonaws.com/thumbnails/{document.UserId}/{document.Id}";

            var databaseResponse = await LoadDocument(document.Id);
            databaseResponse.Thumbnail.Should().Be(expectedThumbnailUrl);
        }

        [Fact]
        public async Task DeleteAllDocuments_WhenNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act
            Func<Task> func = async () => await _gateway.DeleteAllDocuments(userId, directoryId);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAllDocuments_WhenCalled_DeletesAllDocumentsFromDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            var numberOfDocuments = _random.Next(2, 5);

            var documents = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, directoryId)
                .CreateMany(numberOfDocuments)
                .ToList();

            await AddDocumentsToDb(documents);

            // Act
            await _gateway.DeleteAllDocuments(userId, directoryId);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Documents
                .Where(x => x.UserId == userId && x.DirectoryId == directoryId)
                .ToListAsync();

            dbResponse.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllDocuments_WhenNotFound_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act
            var response = await _gateway.GetAllDocuments(userId, directoryId);

            // Assert
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllDocuments_WhenCalled_DeletesAllDocumentsFromDatabase()
        {
            // Arrange
            var directory = _fixture.Create<DirectoryDb>();
            await AddDirectoryToDb(directory);

            var numberOfDocuments = _random.Next(2, 5);

            var documents = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, directory.UserId)
                .With(x => x.DirectoryId, directory.Id)
                .Without(x => x.Directory)
                .CreateMany(numberOfDocuments).ToList();

            await AddDocumentsToDb(documents);

            // Act
            var response = await _gateway.GetAllDocuments(directory.UserId, directory.Id);

            // Assert
            response.Should().HaveCount(numberOfDocuments);
        }

        private static async Task AddDirectoryToDb(DirectoryDb directory)
        {
            InMemoryDb.Instance.Directories.Add(directory);
            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private static async Task AddDocumentsToDb(List<DocumentDb> documents)
        {
            InMemoryDb.Instance.Documents.AddRange(documents);
            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private async Task<DocumentDb> LoadDocument(Guid id)
        {
            return await InMemoryDb.Instance.Documents.FindAsync(id);
        }

        private async Task SetupTestData(DocumentDb document)
        {
            InMemoryDb.Instance.Documents.Add(document);

            await InMemoryDb.Instance.SaveChangesAsync();
        }
    }
}
