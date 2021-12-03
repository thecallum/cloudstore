﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using AutoFixture;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentServiceListener.Tests
{
    [Collection("Database collection")]
    public class BaseIntegrationTest : IDisposable
    {
        protected readonly IAmazonDynamoDB _client;
        protected readonly IDynamoDBContext _context;
        protected readonly IAmazonS3 _s3Client;

        protected readonly DatabaseFixture _testFixture;
        protected readonly HttpClient _httpClient;

        protected readonly Fixture _fixture = new Fixture();
        protected readonly Random _random = new Random();
        protected readonly List<Action> _cleanup = new List<Action>();

        protected readonly User _user;
        protected readonly string _token;

        public BaseIntegrationTest(DatabaseFixture testFixture)
        {
            _client = testFixture.DynamoDb;
            _context = testFixture.DynamoDbContext;

            _testFixture = testFixture;
            _httpClient = testFixture.Client;

            _s3Client = testFixture.S3Client;

            _user = ContextHelper.CreateUser();
            _token = ContextHelper.CreateToken(_user);
        }

        public void Dispose()
        {
            foreach (var action in _cleanup) action();
        }

        protected async Task SetupTestData(DocumentDb document)
        {
            await _context.SaveAsync<DocumentDb>(document);

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentDb>(document.UserId, document.DocumentId));
        }

        protected async Task SetupTestData(DocumentStorageDb entity)
        {
            await _context.SaveAsync<DocumentStorageDb>(entity);

            _cleanup.Add(async () => await _context.DeleteAsync<DocumentStorageDb>(entity.UserId));
        }

        protected async Task SetupTestData(IEnumerable<DocumentDb> documents)
        {
            foreach (var document in documents)
            {
                await SetupTestData(document);
            }
        }

        protected async Task SetupTestData(DirectoryDb directory)
        {
            await _context.SaveAsync(directory).ConfigureAwait(false);

            _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(directory.UserId, directory.DirectoryId));
        }

        protected async Task SetupTestData(IEnumerable<DirectoryDb> directories)
        {
            var taskList = new List<Task>();

            foreach (var directory in directories) taskList.Add(SetupTestData(directory));

            await Task.WhenAll(taskList);
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
