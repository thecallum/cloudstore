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
using DocumentService.Tests.Infrastructure;
using DocumentService.Tests.Helpers;

namespace DocumentService.Tests
{
    public class UserMockWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }

        public IAmazonS3 S3Client { get; private set; }

        public string ValidFilePath { get; private set; }
        public string TooLargeFilePath { get; private set; }

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

                EnsureBucketExists();
                CreateTestFiles();
            });
        }

        private void CreateTestFiles()
        {
            TooLargeFilePath = TestHelpers.GetFilePath("tooLargeFile.txt", 1073741825);
            ValidFilePath = TestHelpers.GetFilePath("validfile.txt", 200);
        }

        private void EnsureBucketExists()
        {
            try
            {
                S3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = "uploadfromcs",

                }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // table exists
            }
        }

    }
}
