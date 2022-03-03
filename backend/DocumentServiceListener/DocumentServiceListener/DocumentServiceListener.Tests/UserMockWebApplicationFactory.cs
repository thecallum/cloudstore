using Amazon.S3;
using Amazon.S3.Model;
using AWSServerless1;
using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Tests.Helpers;
using DocumentServiceListener.UseCase;
using DocumentServiceListener.UseCase.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Tests
{
    public class UserMockWebApplicationFactory
    {
        private readonly string _connection = null;
        public IAmazonS3 S3Client { get; private set; }

        public string ValidFilePath { get; private set; }
        public string TooLargeFilePath { get; private set; }

        private IServiceProvider _services;

        public UserMockWebApplicationFactory()
        { }

        public IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
          .ConfigureServices((hostContext, services) =>
          {

              services.AddSingleton<IConfiguration>(hostContext.Configuration);

              services.ConfigureAws();

              services.RemoveAll(typeof(DbContextOptions<DocumentServiceContext>));

              ConfigureDbContext(services);

              var serviceProvider = services.BuildServiceProvider();

              _services = serviceProvider;

              InitialiseDB(serviceProvider);

              S3Client = serviceProvider.GetRequiredService<IAmazonS3>();

              EnsureBucketExists();
              CreateTestFiles();
          });

        protected void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<DocumentServiceContext>(options =>
            {
                options.UseInMemoryDatabase("integration");
                options.ConfigureWarnings(warningOptions =>
                {
                    warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                });
            });
        }

        private void CreateTestFiles()
        {
            TooLargeFilePath = TestHelpers.GenerateTestFile("tooLargeFile.txt", 1000);
            ValidFilePath = TestHelpers.GenerateTestFile("validfile.txt", 200);
        }

        private void EnsureBucketExists()
        {
            Environment.SetEnvironmentVariable("S3_BUCKET_NAME", "uploadfromcs");
            Environment.SetEnvironmentVariable("S3_BUCKET_BASE_PATH", "https://uploadfromcs.s3.eu-west-1.amazonaws.com/thumbnails");

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

        private static void InitialiseDB(ServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DocumentServiceContext>();

            dbContext.Database.EnsureCreated();
            dbContext.SaveChanges();
        }

        public ScopedContext GetContext()
        {
            return new ScopedContext(_services);
        }
    }

    public sealed class ScopedContext : IDisposable
    {
        private readonly IServiceScope _scope;

        public DocumentServiceContext DB { get; private set; }

        public ScopedContext(IServiceProvider services)
        {
            _scope = services.CreateScope();
            DB = _scope.ServiceProvider.GetRequiredService<DocumentServiceContext>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
