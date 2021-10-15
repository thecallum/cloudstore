using DocumentService.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;

namespace DocumentService.Tests.Gateways
{
    public class StorageServiceGatewayTests : BaseIntegrationTest
    {
        private readonly StorageServiceGateway _gateway;

        public StorageServiceGatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _gateway = new StorageServiceGateway(_context);
        }

        [Fact]
        public async Task CanUploadFile_WhenEntityDoesntExist_ReturnsTrue()
        {
            // Arrange
            long accountStorageCapacity = _fixture.Create<long>();
            var fileSize = accountStorageCapacity - 10;

            // Act
            var response = await _gateway.CanUploadFile(_userId, accountStorageCapacity, fileSize);

            // Assert
            response.Should().BeTrue();
        }

        [Theory]
        [InlineData(80, 30)]
        [InlineData(80, 30, 5)]
        public async Task CapUploadFile_WhenFileTooLarge_ReturnsFalse(long storageUsage, long fileSize, long? originalFileSize = null)
        {
            // Arrange
            long accountStorageCapacity = 100;

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };

            await SetupTestData(entity);

            // Act
            var response = await _gateway.CanUploadFile(_userId, accountStorageCapacity, fileSize, originalFileSize);

            // Assert
            response.Should().BeFalse();
        }

        [Theory]
        [InlineData(80, 10)]
        [InlineData(80, 30, 50)]
        public async Task CanUploadFile_WhenValidSize_ReturnsTrue(long storageUsage, long fileSize, long? originalFileSize = null)
        {
            // Arrange
            long accountStorageCapacity = 100;

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };

            await SetupTestData(entity);

            // Act
            var response = await _gateway.CanUploadFile(_userId, accountStorageCapacity, fileSize, originalFileSize);

            // Assert
            response.Should().BeTrue();
        }

        [Fact]
        public async Task AddFile_WhenEntityDoesntExist_CreatesNewEntity()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();

            // Act
            await _gateway.AddFile(_userId, fileSize);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentStorageDb>(_userId);

            databaseResponse.StorageUsage.Should().Be(fileSize);
        }

        [Fact]
        public async Task AddFile_WhenEntityExists_UpdatesUsage()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();
            var storageUsage = _fixture.Create<long>();

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };
            await SetupTestData(entity);

            // Act
            await _gateway.AddFile(_userId, fileSize);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentStorageDb>(_userId);

            databaseResponse.StorageUsage.Should().Be(fileSize + storageUsage);
        }

        [Fact]
        public async Task GetUsage_WhenEntityDoesntExist_ReturnsZero()
        {
            // Arrange
            var accountStorageCapacity = _fixture.Create<long>();

            // Act
            var response = await _gateway.GetUsage(_userId, accountStorageCapacity);

            // Assert
            response.Capacity.Should().Be(accountStorageCapacity);
            response.StorageUsage.Should().Be(0);
        }

        [Fact]
        public async Task GetUsage_WhenEntityExists_ReturnsUsage()
        {
            // Arrange
            var accountStorageCapacity = _fixture.Create<long>();
            var storageUsage = _fixture.Create<long>();

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };
            await SetupTestData(entity);

            // Act
            var response = await _gateway.GetUsage(_userId, accountStorageCapacity);

            // Assert
            response.Capacity.Should().Be(accountStorageCapacity);
            response.StorageUsage.Should().Be(storageUsage);
        }

        [Fact]
        public async Task RemoveFile_WhenEntityDoesntExist_ThrowsException()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();

            // Act
            Func<Task> func = async () => await _gateway.RemoveFile(_userId, fileSize);

            // Assert
            await func.Should().ThrowAsync<StorageUsageNotFoundException>();
        }

        [Fact]
        public async Task RemoveFile_WhenEntityExists_UpdatesUsageInDatabase()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();
            var storageUsage = _fixture.Create<long>();

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };
            await SetupTestData(entity);

            // Act
            await _gateway.RemoveFile(_userId, fileSize);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentStorageDb>(_userId);

            databaseResponse.StorageUsage.Should().Be(storageUsage - fileSize);
        }

        [Fact]
        public async Task ReplaceFile_WhenEntityDoesntExist_ThrowsException()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();
            var originalFileSize = _fixture.Create<long>();

            // Act
            Func<Task> func = async () => await _gateway.ReplaceFile(_userId, fileSize, originalFileSize);

            // Assert
            await func.Should().ThrowAsync<StorageUsageNotFoundException>();

        }

        [Fact]
        public async Task ReplaceFile_WhenEntityExists_UpdatesUsageInDatabase()
        {
            // Arrange
            var fileSize = _fixture.Create<long>();
            var originalFileSize = _fixture.Create<long>();
            var storageUsage = _fixture.Create<long>();

            var entity = new DocumentStorageDb { UserId = _userId, StorageUsage = storageUsage };
            await SetupTestData(entity);

            // Act
            await _gateway.ReplaceFile(_userId, fileSize, originalFileSize);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentStorageDb>(_userId);

            databaseResponse.StorageUsage.Should().Be(storageUsage + originalFileSize - fileSize);
        }
    }
}
