using Amazon.S3;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests
{
    public class DatabaseFixture
    {
        private readonly UserMockWebApplicationFactory _factory;
        private readonly IHost _host;

        public ScopedContext GetContext()
        {
            return _factory.GetContext();
        }

        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("SECRET", "abcdefg");

            _factory = new UserMockWebApplicationFactory();
            _host = _factory.CreateHostBuilder(null).Build();
            _host.Start();
        }


        public HttpClient Client { get; }

        public IAmazonS3 S3Client => _factory.S3Client;


        public string ValidFilePath => _factory.ValidFilePath;
        public string TooLargeFilePath => _factory.TooLargeFilePath;

        public IConnectionMultiplexer Redis => _factory.Redis;

        public void Dispose()
        {
            InMemoryDb.Teardown();
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
