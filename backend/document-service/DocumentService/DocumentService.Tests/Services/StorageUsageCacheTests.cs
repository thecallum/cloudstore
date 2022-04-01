using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Services;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Services
{
    public class StorageUsageCacheTests : BaseIntegrationTest
    {
        private readonly StorageUsageCache _classUnderTest;
        private readonly IDatabase _database;

        public StorageUsageCacheTests(DatabaseFixture<Startup> testFixture)
           : base(testFixture)
        {
            _database = testFixture.Redis.GetDatabase();

            _classUnderTest = new StorageUsageCache(testFixture.Redis);
        }

        [Fact]
        public async Task GetValue_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _classUnderTest.GetValue(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetValue_WhenFound_ReturnsValue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _fixture.Create<long>();

            await _database.StringGetSetAsync($"usage:{userId}", amount);

            // Act
            var result = await _classUnderTest.GetValue(userId);

            // Assert
            result.Should().Be(amount);
        }

        [Fact]
        public async Task UpdateValue_WhenCalled_SetsValue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _fixture.Create<long>();

            // Act
            await _classUnderTest.UpdateValue(userId, amount);

            // Assert
            var dbResponse = await _database.StringGetAsync($"usage:{userId}");
            dbResponse.Should().Be(amount);
        }
    }
}
