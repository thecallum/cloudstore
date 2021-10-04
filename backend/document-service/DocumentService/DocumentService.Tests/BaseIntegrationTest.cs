using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using AutoFixture;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests
{
    [Collection("Database collection")]
    public class BaseIntegrationTest : IDisposable
    {
        protected readonly IAmazonDynamoDB _client;
        protected readonly IDynamoDBContext _context;
        protected readonly IAmazonS3 _s3Client;

        protected readonly DatabaseFixture<Startup> _testFixture;
        protected readonly HttpClient _httpClient;

        protected readonly Fixture _fixture = new Fixture();
        protected readonly Random _random = new Random();
        protected readonly List<Action> _cleanup = new List<Action>();

        public BaseIntegrationTest(DatabaseFixture<Startup> testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;
            _httpClient = testFixture.Client;

            _s3Client = testFixture.S3Client;
        }

        public void Dispose()
        {
            foreach (var action in _cleanup) action();
        }

        protected async Task SetupTestData(DocumentDb document)
        {
            await _context.SaveAsync<DocumentDb>(document);

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(document.UserId, document.DocumentId));
        }

        protected async Task SetupTestData(IEnumerable<DocumentDb> documents)
        {
            foreach (var document in documents)
            {
                await SetupTestData(document);
            }
        }

        protected async Task SetupTestData(DirectoryDb directory)
        {
            await _context.SaveAsync(directory).ConfigureAwait(false);

            _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(directory.UserId, directory.DirectoryId));
        }

        protected async Task SetupTestData(IEnumerable<DirectoryDb> directories)
        {
            var taskList = new List<Task>();

            foreach (var directory in directories) taskList.Add(SetupTestData(directory));

            await Task.WhenAll(taskList);
        }

        protected JsonSerializerOptions CreateJsonOptions()
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
