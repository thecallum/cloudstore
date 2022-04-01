using AutoFixture;
using DocumentServiceListener.Services;
using FluentAssertions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentServiceListener.Tests.Services
{
    [Collection("Database collection")]
    public class StorageUsageCacheTests
    {
        private readonly IDatabase _database;
        private readonly StorageUsageCache _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public StorageUsageCacheTests(DatabaseFixture testFixture)
        {
            _database = testFixture.Redis.GetDatabase();

            _classUnderTest = new StorageUsageCache(testFixture.Redis);
        }

        [Fact]
        public async Task DeleteCache_WhenObjectNotFound_DoesntThrowError()
        {
            // Arrange
            var user = _fixture.Create<User>();

            // Act
            Func<Task> func = async () => await _classUnderTest.DeleteCache(user);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteCache_WhenObjectExists_DeletesObjectFromCache()
        {
            // Arrange
            var user = _fixture.Create<User>();

            var expectedKey = $"usage:{user.Id}";

            await _database.StringSetAsync(expectedKey, "1");

            // Act
            await _classUnderTest.DeleteCache(user);

            // Assert
            var dbResponse = await _database.StringGetAsync(expectedKey);
            dbResponse.IsNull.Should().BeTrue();
        }
    }
}
