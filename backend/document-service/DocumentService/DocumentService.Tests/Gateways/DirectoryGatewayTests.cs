using AutoFixture;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
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

        private async Task SetupTestData(DirectoryDb directory)
        {
            InMemoryDb.Instance.Directories.Add(directory);

            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private async Task SetupTestData(IEnumerable<DirectoryDb> directories)
        {
            foreach(var directory in directories)
            {
                await SetupTestData(directory);
            }
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        [Fact]
        public async Task CreateDirectory_WhenCalled_IsSavedToDatabase()
        {
            // Arrange
            var newDirectory = _fixture.Create<DirectoryDomain>();

            // Act 
            await _gateway.CreateDirectory(newDirectory);

            // Assert
           // var databaseResponse = await _context.LoadAsync<DirectoryDb>(newDirectory.UserId, newDirectory.DirectoryId);
            var databaseResponse = await InMemoryDb.Instance.Directories.FindAsync(newDirectory.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(newDirectory.Name);
            databaseResponse.ParentDirectoryId.Should().Be(newDirectory.ParentDirectoryId);

            //_cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(newDirectory.UserId, newDirectory.DirectoryId));
        }

        [Fact]
        public async Task DeleteDirectory_WhenDirectoryDoesntExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act 
            Func<Task> func = async () => await _gateway.DeleteDirectory(directoryId, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }


        [Fact]
        public async Task DeleteDirectory_WhenValid_DeletesDirectoryFromDatabase()
        {
            // Arrange
            var mockDirectory = _fixture.Create<DirectoryDomain>().ToDatabase(null);

            await SetupTestData(mockDirectory);

            // Act 
            await _gateway.DeleteDirectory(mockDirectory.Id, mockDirectory.UserId);

            // Assert
            var databaseResponse = await InMemoryDb.Instance.Directories.FindAsync(mockDirectory.Id);

            databaseResponse.Should().BeNull();
        }

        [Fact]
        public async Task RenameDirectory_WhenDirectoryDoesntExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();
            var newName = _fixture.Create<string>();

            // Act 
            Func<Task> func = async () => await _gateway.RenameDirectory(newName, directoryId, userId);

            // Assert
            await func.Should().ThrowAsync<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task RenameDirectory_WhenValid_UpdatesDirectoryInDatabase()
        {
            // Arrange

            var mockDirectory = _fixture.Create<DirectoryDomain>().ToDatabase(null);

            await SetupTestData(mockDirectory);

            var newName = _fixture.Create<string>();

            // Act 
            await _gateway.RenameDirectory(newName, mockDirectory.Id, mockDirectory.UserId);

            // Assert

//            var databaseResponse = await _context.LoadAsync<DirectoryDb>(mockDirectory.UserId, mockDirectory.Id);
            var databaseResponse = await InMemoryDb.Instance.Directories.FindAsync(mockDirectory.Id);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(newName);

          //  _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(mockDirectory.UserId, mockDirectory.DirectoryId));

        }

        [Fact]
        public async Task GetAllDirectories_WhenNoneExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act 
            var response = await _gateway.GetAllDirectories(userId);

            // Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDirectories_WhenManyExist_ReturnsMany()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var childParentDirectoryId = Guid.NewGuid();

            var numberOfDirectoriesInRootDirectory = _random.Next(2, 5);
            var numberOfDirectoriesChildDirectory = _random.Next(2, 5);

            var directoriesInRootDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.ParentDirectoryId, userId)
                .With(x => x.UserId, userId)
                .CreateMany(numberOfDirectoriesInRootDirectory)
                .Select(x => x.ToDatabase(null));

            var directoriesInChildDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.ParentDirectoryId, childParentDirectoryId)
                .With(x => x.UserId, userId)
                .CreateMany(numberOfDirectoriesChildDirectory)
                .Select(x => x.ToDatabase(null));

            await SetupTestData(directoriesInRootDirectory);
            await SetupTestData(directoriesInChildDirectory);

            // Act 
            var response = await _gateway.GetAllDirectories(userId);

            // Assert
            response.Should().HaveCount(numberOfDirectoriesInRootDirectory + numberOfDirectoriesChildDirectory);
        }

        [Fact]
        public async Task CheckDirectoryExists_WhenItDoesntExist_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var directoryId = Guid.NewGuid();

            // Act 
            var response = await _gateway.CheckDirectoryExists(directoryId, userId);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task CheckDirectoryExists_WhenItExists_ReturnsTrue()
        {
            // Arrange
            var directory = _fixture.Create<DirectoryDomain>()
                .ToDatabase(null);

            await SetupTestData(directory);

            // Act 
            var response = await _gateway.CheckDirectoryExists(directory.Id, directory.UserId);

            // Assert
            response.Should().BeTrue();
        }

        [Fact]
        public async Task ContainsChildDirectories_WhenFalse_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var parentDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, userId)
                .Create()
                .ToDatabase(null);

            await SetupTestData(parentDirectory);

            // Act
            var response = await _gateway.ContainsChildDirectories(parentDirectory.Id, userId);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public async Task ContainsChildDirectories_WhenTrue_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var parentDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, userId)
                .Create()
                .ToDatabase(null);

            await SetupTestData(parentDirectory);

            var childDirectory = _fixture.Build<DirectoryDomain>()
                .With(x => x.UserId, userId)
                .With(x => x.ParentDirectoryId, parentDirectory.Id)
                .Create()
                .ToDatabase(null);

            await SetupTestData(childDirectory);

            // Act
            var response = await _gateway.ContainsChildDirectories(parentDirectory.Id, userId);

            // Assert
            response.Should().BeTrue();
        }
    }
}
