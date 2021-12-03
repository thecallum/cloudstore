using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DocumentServiceListener.Tests.Intfrastructure;

namespace DocumentServiceListener.Tests
{
    public class DatabaseFixture
    {
        private readonly UserMockWebApplicationFactory _factory;
        private readonly IHost _host;

        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");

            _factory = new UserMockWebApplicationFactory();
            _host = _factory.CreateHostBuilder(null).Build();
            _host.Start();

            ResetDatabase().GetAwaiter().GetResult();
        }

        private async Task ResetDatabase()
        {
            var tables = await DynamoDb.ListTablesAsync();

            if (tables.TableNames.Contains("Document"))
            {
                await DynamoDb.DeleteTableAsync("Document");
            }

            if (tables.TableNames.Contains("Directory"))
            {
                await DynamoDb.DeleteTableAsync("Directory");
            }

            if (tables.TableNames.Contains("DocumentStorage"))
            {
                await DynamoDb.DeleteTableAsync("DocumentStorage");
            }

            await DatabaseBuilder.CreateDocumentTable(DynamoDb);
            await DatabaseBuilder.CreateDirectoryTable(DynamoDb);
            await DatabaseBuilder.CreateDocumentStorageTable(DynamoDb);
        }

        public HttpClient Client { get; }

        public IAmazonDynamoDB DynamoDb => _factory.DynamoDb;
        public IDynamoDBContext DynamoDbContext => _factory.DynamoDbContext;

        public IAmazonS3 S3Client => _factory.S3Client;

        public string ValidFilePath => _factory.ValidFilePath;
        public string TooLargeFilePath => _factory.TooLargeFilePath;


        public void Dispose()
        {
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }
    }

    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
