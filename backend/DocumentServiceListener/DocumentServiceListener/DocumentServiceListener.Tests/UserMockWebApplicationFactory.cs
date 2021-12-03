using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Tests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Tests
{
    public class UserMockWebApplicationFactory
    {
        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }

        public string ValidFilePath { get; private set; }
        public string TooLargeFilePath { get; private set; }


        public IAmazonS3 S3Client { get; private set; }

        public IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
          .ConfigureServices((hostContext, services) =>
          {
              services.ConfigureAws();

              var serviceProvider = services.BuildServiceProvider();
              DynamoDb = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
              DynamoDbContext = serviceProvider.GetRequiredService<IDynamoDBContext>();
              S3Client = serviceProvider.GetRequiredService<IAmazonS3>();

              EnsureBucketExists();
              CreateTestFiles();
          });

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

        private void CreateTestFiles()
        {
            TooLargeFilePath = TestHelpers.GetFilePath("tooLargeFile.txt", 1000);
            ValidFilePath = TestHelpers.GetFilePath("validfile.txt", 200);
        }

    }
}
