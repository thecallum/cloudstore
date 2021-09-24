using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using authservice.Infrastructure;
using System.Net.Http;
using Microsoft.AspNetCore;

namespace authservice.Tests
{
    public class DatabaseFixture<TStartup> : IDisposable where TStartup : class
    {
        public HttpClient Client { get; private set; }

        public IAmazonDynamoDB DynamoDb => _factory.DynamoDb;
        public IDynamoDBContext DynamoDbContext => _factory.DynamoDbContext;

        private const string UserTableName = "User";

        private readonly UserMockWebApplicationFactory<TStartup> _factory;
   
        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("SECRET", "abcdefg");

            _factory = new UserMockWebApplicationFactory<TStartup>();
            Client = _factory.CreateClient();
        }   

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        public void Dispose()
        {
            // cleanup database
            DynamoDb.DeleteTableAsync(UserTableName).GetAwaiter().GetResult();
        }

        public async Task ResetDatabase()
        {
            await DynamoDb.DeleteTableAsync(UserTableName);
            await _factory.CreateUserTable();
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
