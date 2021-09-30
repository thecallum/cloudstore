using AutoFixture;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using Directory = DocumentService.Domain.Directory;

namespace DocumentService.Tests.Gateways
{
    public class DirectoryGatewayTests : BaseIntegrationTest
    {
        private readonly DirectoryGateway _gateway;

        public DirectoryGatewayTests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _gateway = new DirectoryGateway(_context);
        }

        [Fact]
        public async Task CreateDirectory_WhenCalled_IsSavedToDatabase()
        {
            // Arrange
            var newDirectory = _fixture.Create<Directory>();

            // Act 
            await _gateway.CreateDirectory(newDirectory);

            // Assert
            var databaseResponse = await _context.LoadAsync<DirectoryDb>(newDirectory.UserId, newDirectory.DirectoryId);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(newDirectory.Name);
            databaseResponse.ParentDirectoryId.Should().Be(newDirectory.ParentDirectoryId);

            _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(newDirectory.UserId, newDirectory.DirectoryId));
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
            var mockDirectory = _fixture.Create<DirectoryDb>();

            await SetupTestData(mockDirectory);

            // Act 
            await _gateway.DeleteDirectory(mockDirectory.DirectoryId, mockDirectory.UserId);

            // Assert
            var databaseResponse = await _context.LoadAsync<DirectoryDb>(mockDirectory.UserId, mockDirectory.DirectoryId);

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

            var mockDirectory = _fixture.Create<DirectoryDb>();

            await SetupTestData(mockDirectory);

            var newName = _fixture.Create<string>();

            // Act 
            await _gateway.RenameDirectory(newName, mockDirectory.DirectoryId, mockDirectory.UserId);

            // Assert

            var databaseResponse = await _context.LoadAsync<DirectoryDb>(mockDirectory.UserId, mockDirectory.DirectoryId);

            databaseResponse.Should().NotBeNull();
            databaseResponse.Name.Should().Be(newName);

            _cleanup.Add(async () => await _context.DeleteAsync<DirectoryDb>(mockDirectory.UserId, mockDirectory.DirectoryId));

        }

        [Fact]
        public async Task GetAllDirectories_WhenNoneExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var parentDirectoryId = Guid.NewGuid();

            // Act 
            var response = await _gateway.GetAllDirectories(userId, parentDirectoryId);

            // Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAllDirectories_WhenParentDirectoryIsNull_ReturnsRootDirectories()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var childParentDirectoryId = Guid.NewGuid();

            var numberOfDirectoriesInRootDirectory = _random.Next(2, 5);
            var numberOfDirectoriesChildDirectory = numberOfDirectoriesInRootDirectory + 1;

            var directoriesInRootDirectory = _fixture.Build<DirectoryDb>()
                                                .With(x => x.ParentDirectoryId, userId)
                                                .With(x => x.UserId, userId)
                                                .CreateMany(numberOfDirectoriesInRootDirectory);

            var directoriesInChildDirectory = _fixture.Build<DirectoryDb>()
                                              .With(x => x.ParentDirectoryId, childParentDirectoryId)
                                              .With(x => x.UserId, userId)
                                              .CreateMany(numberOfDirectoriesChildDirectory);

            await SetupTestData(directoriesInRootDirectory);
            await SetupTestData(directoriesInChildDirectory);

            // Act 
            var response = await _gateway.GetAllDirectories(userId, null);

            // Assert
            response.Should().HaveCount(numberOfDirectoriesInRootDirectory);
        }

        [Fact]
        public async Task GetAllDirectories_WhenParentDirectoryIsNotNull_ReturnsChildDirectories()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var childParentDirectoryId = Guid.NewGuid();

            var numberOfDirectoriesInRootDirectory = _random.Next(2, 5);
            var numberOfDirectoriesChildDirectory = numberOfDirectoriesInRootDirectory + 1;

            var directoriesInRootDirectory = _fixture.Build<DirectoryDb>()
                                                .With(x => x.ParentDirectoryId, userId)
                                                .With(x => x.UserId, userId)
                                                .CreateMany(numberOfDirectoriesInRootDirectory);

            var directoriesInChildDirectory = _fixture.Build<DirectoryDb>()
                                              .With(x => x.ParentDirectoryId, childParentDirectoryId)
                                              .With(x => x.UserId, userId)
                                              .CreateMany(numberOfDirectoriesChildDirectory);

            await SetupTestData(directoriesInRootDirectory);
            await SetupTestData(directoriesInChildDirectory);

            // Act 
            var response = await _gateway.GetAllDirectories(userId, childParentDirectoryId);

            // Assert
            response.Should().HaveCount(numberOfDirectoriesChildDirectory);
        }
    }
}
