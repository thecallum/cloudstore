using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateways
{
    public class DocumentGatewayTests : BaseIntegrationTest
    {
        private readonly IDocumentGateway _gateway;

        public DocumentGatewayTests(DatabaseFixture testFixture)
            : base(testFixture)
        {
            _gateway = new DocumentGateway(_context);
        }

        [Fact]
        public async Task DeleteDocuments_WhenNoDocumentsExist_DoesntThrowError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documents = new List<DocumentDb>();

            // Act
            await _gateway.DeleteDocuments(documents, userId);

            // Assert
            
            // Will fail if exception is thrown
        }

        [Fact]
        public async Task DeleteDocuments_WhenCalled_DeletesManyDocuments()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            var documents = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, directoryId)
                .CreateMany(2).ToList();

            await SetupTestData(documents);

            // Act
            await _gateway.DeleteDocuments(documents, userId);

            // Assert

            var databaseResponse = await GetAllDocuments(directoryId);
            databaseResponse.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenCalled_ReturnsManyDocuments()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            var documents = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .With(x => x.DirectoryId, directoryId)
                .CreateMany(3).ToList();

            await SetupTestData(documents);

            // Act
            var response = await _gateway.GetAllDocuments(directoryId, userId);

            // Assert
            response.Should().HaveCount(3);
        }

        private async Task<List<DocumentDb>> GetAllDocuments(Guid directoryId)
        {
            var documentList = new List<DocumentDb>();

            var config = new DynamoDBOperationConfig
            {
                IndexName = "DirectoryId_Name",
            };

            var search = _context.QueryAsync<DocumentDb>(directoryId, config);

            do
            {
                var newDocuments = await search.GetNextSetAsync();

                documentList.AddRange(newDocuments);

            } while (search.IsDone == false);

            return documentList;
        }
    }
}
