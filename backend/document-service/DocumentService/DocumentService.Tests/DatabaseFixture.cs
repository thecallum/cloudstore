using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests
{
    public class DatabaseFixture<TStartup> : IDisposable where TStartup : class
    {
        private const string DocumentTableName = "Document";
        private readonly UserMockWebApplicationFactory<TStartup> _factory;
        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");

            _factory = new UserMockWebApplicationFactory<TStartup>();
            Client = _factory.CreateClient();
        }

        public HttpClient Client { get; }

        public IAmazonDynamoDB DynamoDb => _factory.DynamoDb;
        public IDynamoDBContext DynamoDbContext => _factory.DynamoDbContext;

        public IAmazonS3 S3Client => _factory.S3Client;

        public void Dispose()
        {
            // cleanup database
            DynamoDb.DeleteTableAsync(DocumentTableName).GetAwaiter().GetResult();
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        public async Task ResetDatabase()
        {
            await DynamoDb.DeleteTableAsync(DocumentTableName);
            await _factory.CreateDocumentTable();
        }
    }

    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
