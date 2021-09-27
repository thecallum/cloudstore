using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocumentService.Gateways;
using Amazon.S3;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DocumentService.Infrastructure;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;

namespace DocumentService.Tests
{
    public class UserMockWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        private const string DocumentTableName = "Document";
        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }

        public IAmazonS3 S3Client { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();

            builder.ConfigureServices(services =>
            {
                services.ConfigureAws();

                var serviceProvider = services.BuildServiceProvider();
                DynamoDb = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
                DynamoDbContext = serviceProvider.GetRequiredService<IDynamoDBContext>();
                S3Client = serviceProvider.GetRequiredService<IAmazonS3>();

                EnsureTableExist();

                S3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = "uploadfromcs",

                }).GetAwaiter().GetResult();

            });
        }

        private void EnsureTableExist()
        {
            // initialise data in the test database
            try
            {
                CreateDocumentTable().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                // table exists
            }
        }

        public async Task CreateDocumentTable()
        {
            var request = new CreateTableRequest
            {
                TableName = DocumentTableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "userId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "id",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "userId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "id",
                        KeyType = "Range" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                }
            };

            await DynamoDb.CreateTableAsync(request).ConfigureAwait(false);
        }
    }

}
