using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Tests.Helpers;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    [Collection("Database collection")]
    public class GetAllDocumentsE2ETests : IDisposable
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        public GetAllDocumentsE2ETests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;

            _httpClient = testFixture.Client;


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
        public async Task GetAllDocuments_WhenNoDocumentsExist_ReturnsEmptyList()
        {
            // Arrange

            // Act
            var response = await GetAllDocumentsRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<GetAllDocumentsResponse>(responseContent, CreateJsonOptions());

            uploadDocumentResponse.Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDocuments_WhenManyDocumentsExist_ReturnsList()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var numberOfDocuments = _random.Next(2, 5);
            var mockDocuments = _fixture
                .Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .CreateMany(numberOfDocuments);

            foreach (var document in mockDocuments)
            {
                await SetupTestData(document);
            }

            // Act
            var response = await GetAllDocumentsRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<GetAllDocumentsResponse>(responseContent, CreateJsonOptions());

            uploadDocumentResponse.Documents.Should().HaveCount(numberOfDocuments);
        }


        private async Task<HttpResponseMessage> GetAllDocumentsRequest()
        {
            var uri = new Uri("/api/document", UriKind.Relative);

            //var json = JsonConvert.SerializeObject(request);
           // var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return response;
        }

        private JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
