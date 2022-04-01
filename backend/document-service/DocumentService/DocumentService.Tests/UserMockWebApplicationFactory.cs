using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using DocumentService.Infrastructure;
using Amazon.S3.Model;
using DocumentService.Tests.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace DocumentService.Tests
{
    public class UserMockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string _connection = null;
        public IAmazonS3 S3Client { get; private set; }
        public IConnectionMultiplexer Redis { get; private set; }

        public string ValidFilePath { get; private set; }
        public string TooLargeFilePath { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.ConfigureAws();
                Redis = services.ConfigureRedis();

                services.RemoveAll(typeof(DbContextOptions<DocumentServiceContext>));

                services.AddDbContext<DocumentServiceContext>(options =>
                {
                    if (_connection != null)
                    {
                        options.UseNpgsql(_connection);
                    }
                    else
                    {
                        options.UseInMemoryDatabase("integration");
                        options.ConfigureWarnings(warningOptions =>
                        {
                            warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                        });
                    }
                });

                var serviceProvider = services.BuildServiceProvider();
                InitialiseDB(serviceProvider);

                S3Client = serviceProvider.GetRequiredService<IAmazonS3>();

                EnsureBucketExists();
                CreateTestFiles();

            });
        }

        private void CreateTestFiles()
        {
            TooLargeFilePath = TestHelpers.GetFilePath("tooLargeFile.txt", 1000);
            ValidFilePath = TestHelpers.GetFilePath("validfile.txt", 200);
        }

        private void EnsureBucketExists()
        {
            Environment.SetEnvironmentVariable("S3_BUCKET_NAME", "uploadfromcs");

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
            return new ScopedContext(Services);
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
