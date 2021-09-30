using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
{
    [Collection("Database collection")]
    public class DocumentGatewayTests : IDisposable
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly Fixture _fixture = new Fixture();

        private readonly DocumentGateway _gateway;
        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        public DocumentGatewayTests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _gateway = new DocumentGateway(_context);

            _testFixture = testFixture;
        }

        public void Dispose()
        {
            _testFixture.ResetDatabase().GetAwaiter().GetResult();
        }

        private async Task SetupTestData(DocumentDb document)
        {
            await _context.SaveAsync(document).ConfigureAwait(false);
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
                .CreateMany(numberOfDocuments);

            foreach(var document in mockDocuments)
            {
                await SetupTestData(document);
            }

            // Act 
            var response = await _gateway.GetAllDocuments(userId);

            // Assert
            response.Should().HaveCount(numberOfDocuments);
        }
    }
}

