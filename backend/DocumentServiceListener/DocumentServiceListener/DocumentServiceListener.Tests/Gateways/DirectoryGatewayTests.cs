using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;

namespace DocumentServiceListener.Tests.Gateways
{
    public class DirectoryGatewayTests : BaseIntegrationTest
    {
        private readonly IDirectoryGateway _gateway;

        public DirectoryGatewayTests(DatabaseFixture testFixture)
            : base(testFixture)
        {
            _gateway = new DirectoryGateway(_context);
        }

        [Fact]
        public async Task DeleteDirectory_WhenEntityDoesntExist_DoesntThrowError()
        {
            // Arrange
            var directoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            await _gateway.DeleteDirectory(directoryId, userId);

            // Assert
            
            // Fails if exception is thrown
        }

        [Fact]
        public async Task DeleteDirectory_WhenEntityExists_DeletesEntityFromDatabase()
        {
            // Arrange
            var directory = _fixture.Create<DirectoryDb>();

            await SetupTestData(directory);

            // Act
            await _gateway.DeleteDirectory(directory.DirectoryId, directory.UserId);

            // Assert
            var databaseResponse = await _context.LoadAsync<DirectoryDb>(directory.UserId, directory.DirectoryId);
            databaseResponse.Should().BeNull();
        }
    }
}
