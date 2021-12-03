using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateways
{
    public class StorageServiceGatewayTests : BaseIntegrationTest
    {
        private readonly IStorageServiceGateway _gateway;

        public StorageServiceGatewayTests(DatabaseFixture testFixture)
            : base(testFixture)
        {
            _gateway = new StorageServiceGateway(_context);
        }

        [Fact]
        public async Task RemoveDocuments_WhenEntityDoesntExist_ThrowsError()
        {
            // Arrange
            var documents = new List<DocumentDb>();
            var userId = Guid.NewGuid();

            // Act
            Func<Task> func = async () => await _gateway.RemoveDocuments(documents, userId);

            // Assert
            await func.Should().ThrowAsync<StorageUsageNotFoundException>();
        }

        [Fact]
        public async Task RemoveDocuments_WhenCalled_UpdatesStorageUsage()
        {
            // Arrange
            var storageUsage = _fixture.Build<DocumentStorageDb>()
                .With(x => x.StorageUsage, long.MaxValue)
                .Create();

            await SetupTestData(storageUsage);

            var documents = _fixture.CreateMany<DocumentDb>(3).ToList();

            // Act
            await _gateway.RemoveDocuments(documents, storageUsage.UserId);

            // Assert
            var expectedTotal = long.MaxValue;

            foreach (var document in documents) expectedTotal -= document.FileSize;

            var databaseResponse = await _context.LoadAsync<DocumentStorageDb>(storageUsage.UserId);
            databaseResponse.StorageUsage.Should().Be(expectedTotal);
        }
    }
}
