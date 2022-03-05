using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Factories
{
    public class EntityFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void DocumentDomain_ToDatabase()
        {
            // Arrange
            var model = _fixture.Create<DocumentDomain>();

            // Act
            var response = model.ToDatabase();

            // Assert
            response.Id.Should().Be(model.Id);
            response.Name.Should().Be(model.Name);
            response.UserId.Should().Be(model.UserId);
            response.FileSize.Should().Be(model.FileSize);
            response.S3Location.Should().Be(model.S3Location);
            response.DirectoryId.Should().Be(model.DirectoryId);
            response.Thumbnail.Should().Be(model.Thumbnail);
        }

        [Fact]
        public void DocumentDb_ToDomain()
        {
            // Arrange
            var model = new DocumentDb
            {
                Id = _fixture.Create<Guid>(),
                Name = _fixture.Create<string>(),
                UserId = _fixture.Create<Guid>(),
                FileSize = _fixture.Create<long>(),
                S3Location = _fixture.Create<string>(),
                DirectoryId = _fixture.Create<Guid>(),
                Thumbnail = _fixture.Create<string>()
            };

            // Act
            var response = model.ToDomain();

            // Assert
            response.Id.Should().Be(model.Id);
            response.Name.Should().Be(model.Name);
            response.UserId.Should().Be(model.UserId);
            response.FileSize.Should().Be(model.FileSize);
            response.S3Location.Should().Be(model.S3Location);
            response.DirectoryId.Should().Be(model.DirectoryId);
            response.Thumbnail.Should().Be(model.Thumbnail);
        }

        [Fact]
        public void DirectoryDb_ToDomain()
        {
            // Arrange
            var model = new DirectoryDb
            {
                Id = _fixture.Create<Guid>(),
                Name = _fixture.Create<string>(),
                UserId = _fixture.Create<Guid>(),
                ParentDirectoryId = _fixture.Create<Guid?>(),
                ParentDirectoryIds = _fixture.Create<string>()
            };

            // Act
            var response = model.ToDomain();

            // Assert
            response.Id.Should().Be(model.Id);
            response.Name.Should().Be(model.Name);
            response.UserId.Should().Be(model.UserId);
            response.ParentDirectoryId.Should().Be(model.ParentDirectoryId);
        }

        [Fact]
        public void DirectoryDomain_ToDatabase()
        {
            // Arrange
            var model = _fixture.Create<DirectoryDomain>();

            // Act
            var response = model.ToDatabase(null);

            // Assert
            response.Id.Should().Be(model.Id);
            response.Name.Should().Be(model.Name);
            response.UserId.Should().Be(model.UserId);
            response.ParentDirectoryId.Should().Be(model.ParentDirectoryId);
        }

        [Fact]
        public void DirectoryDomain_ToDatabase_WhenRootDirectory_ParentDirectoryIdsIsNull()
        {
            // Arrange
            var model = _fixture.Create<DirectoryDomain>();
            var parentDirectory = null as DirectoryDb;

            // Act
            var response = model.ToDatabase(parentDirectory);

            // Assert
            response.ParentDirectoryIds.Should().BeNull();
        }

        [Fact]
        public void DirectoryDomain_ToDatabase_WhenParentDirectoryIsRoot_ParentDirectoryIdsIsParentDirectoryId()
        {
            // Arrange
            var model = _fixture.Create<DirectoryDomain>();
            var parentDirectory = new DirectoryDb
            {
                Id = Guid.NewGuid(),
                ParentDirectoryId = null,
                ParentDirectoryIds = null
            };

            // Act
            var response = model.ToDatabase(parentDirectory);

            // Assert
            response.ParentDirectoryIds.Should().Be(parentDirectory.Id.ToString());
        }

        [Fact]
        public void DirectoryDomain_ToDatabase_WhenParentDirectoryIsNotRoot_ParentDirectoryIdsContainsManyIds()
        {
            // Arrange
            var model = _fixture.Create<DirectoryDomain>();
            var parentDirectory = new DirectoryDb
            {
                Id = Guid.NewGuid(),
                ParentDirectoryId = Guid.NewGuid(),
                ParentDirectoryIds = $"{Guid.NewGuid()}/{Guid.NewGuid()}/{Guid.NewGuid()}"
            };

            // Act
            var response = model.ToDatabase(parentDirectory);

            // Assert
            var expectedResponse = $"{parentDirectory.ParentDirectoryIds}/{parentDirectory.Id}";
            response.ParentDirectoryIds.Should().Be(expectedResponse);
        }

        [Fact]
        public void CreateDirectoryRequest_ToDomain()
        {
            // Arrange
            var model = _fixture.Create<CreateDirectoryRequest>();
            var userId = Guid.NewGuid();

            // Act
            var response = model.ToDomain(userId);

            // Assert
            response.UserId.Should().Be(userId);
            response.Name.Should().Be(model.Name);
            response.ParentDirectoryId.Should().Be(model.ParentDirectoryId);
        }
    }
}
