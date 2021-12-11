using Amazon.S3;
using AutoFixture;
using DocumentService.Infrastructure;
using DocumentService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentService.Tests
{
    [Collection("Database collection")]
    public class BaseIntegrationTest : IDisposable
    {
        protected readonly IAmazonS3 _s3Client;

        protected readonly DatabaseFixture<Startup> _testFixture;
        protected readonly HttpClient _httpClient;

        protected readonly Fixture _fixture = new Fixture();
        protected readonly Random _random = new Random();
        protected readonly List<Action> _cleanup = new List<Action>();

        protected readonly User _user;
        protected readonly string _token;

        public readonly DatabaseFixture<Startup> _dbFixture;

          private readonly UserMockWebApplicationFactory<Startup> _factory;

        public BaseIntegrationTest(DatabaseFixture<Startup> testFixture)
        {
            _dbFixture = testFixture;
            _httpClient = _dbFixture.Client;

            _s3Client = _dbFixture.S3Client;

            _user = ContextHelper.CreateUser();
            _token = ContextHelper.CreateToken(_user);
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
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

        public async Task<T> DecodeResponse<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseObject = System.Text.Json.JsonSerializer.Deserialize<T>(responseContent, CreateJsonOptions());

            return responseObject;
        }
    }
}
