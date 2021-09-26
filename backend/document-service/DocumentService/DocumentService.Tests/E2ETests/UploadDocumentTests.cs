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
    public class UploadDocumentTests : IDisposable
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly Fixture _fixture = new Fixture();

        private readonly HttpClient _httpClient;

        private readonly Random _random = new Random();

        private readonly DatabaseFixture<Startup> _testFixture;

        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        public UploadDocumentTests(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;

            _httpClient = testFixture.Client;

            _validFilePath = TestHelpers.CreateTestFile("validfile.txt", 200);
            _tooLargeFilePath = TestHelpers.CreateTestFile("tooLargeFile.txt", 1073741825);
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
        public async Task UploadDocument_WhenInvalidFilePath_ReturnsBadRequest()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = "nonExistantFile.txt"
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UploadDocument_WhenFileTooLarge_ReturnsBadRequest()
        {
            // Arrange

            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _tooLargeFilePath
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UploadDocument_WhenValid_ReturnsUploadDocumentResponse()
        {
            // Arrange
            var mockRequest = new UploadDocumentRequest
            {
                FilePath = _validFilePath
            };

            // Act
            var response = await UploadDocumentRequest(mockRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);


            // test contents of response
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var uploadDocumentResponse = System.Text.Json.JsonSerializer.Deserialize<UploadDocumentResponse>(responseContent, CreateJsonOptions());

            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var expectedS3Location = $"{userId}/{uploadDocumentResponse.DocumentId}";
            var expectedFileName = "validfile.txt";

            uploadDocumentResponse.Name.Should().Be(expectedFileName);
            uploadDocumentResponse.S3Location.Should().Be(expectedS3Location);

            // will be set intoken
            var databaseResponse = await _context.LoadAsync<DocumentDb>(userId, uploadDocumentResponse.DocumentId);
            databaseResponse.Should().NotBeNull();

            databaseResponse.UserId.Should().Be(userId);
            databaseResponse.Name.Should().Be(expectedFileName);
            databaseResponse.S3Location.Should().Be(expectedS3Location);
            databaseResponse.FileSize.Should().Be(200);

        }

        private async Task<HttpResponseMessage> UploadDocumentRequest(UploadDocumentRequest request)
        {
            var uri = new Uri("/api/document/upload", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

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
