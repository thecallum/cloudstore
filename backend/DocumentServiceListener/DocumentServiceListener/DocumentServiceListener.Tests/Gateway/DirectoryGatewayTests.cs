using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateway
{
    [Collection("Database collection")]
    public class DirectoryGatewayTests : IDisposable
    {
        private readonly DirectoryGateway _gateway;
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public DirectoryGatewayTests()
        {
            _gateway = new DirectoryGateway(InMemoryDb.Instance);
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        [Fact]
        public async Task DeleteDirectory_WhenNotFound_DoesntThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act
            Func<Task> func = async () => await _gateway.DeleteDirectory(userId, directoryId);

            // Assert
            await func.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteDirectory_WhenCalled_DeletesDirectory()
        {
            // Arrange
            var directory = _fixture.Create<DirectoryDb>();
            await AddDirectoryToDb(directory);

            // Act
            await _gateway.DeleteDirectory(directory.UserId, directory.Id);

            // Assert
            var databaseResponse = await InMemoryDb.Instance.Directories
                .Where(x => x.UserId == directory.UserId && x.Id == directory.Id)
                .SingleOrDefaultAsync();

            databaseResponse.Should().BeNull();
        }

        private static async Task AddDirectoryToDb(DirectoryDb directory)
        {
            InMemoryDb.Instance.Directories.Add(directory);
            await InMemoryDb.Instance.SaveChangesAsync();
        }
    }
}
