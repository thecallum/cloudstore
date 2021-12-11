using AutoFixture;
using DocumentService.Domain;
using DocumentService.Factories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Factories
{
    public class ResponseFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void DirectoryDomainList_ToResponse()
        {
            // Arrange
            var model = _fixture.CreateMany<DirectoryDomain>(3);

            // Act
            var response = model.ToResponse();

            // Assert
            response.Directories.Should().HaveCount(3);
            response.Directories.Should().BeEquivalentTo(model.Select(x => x.ToResponse()));
        }

        [Fact]
        public void DirectoryDomain_ToResponse()
        {
            // Arrange
            var model = _fixture.Create<DirectoryDomain>();

            // Act
            var response = model.ToResponse();

            // Assert
            response.Id.Should().Be(model.Id);
            response.Name.Should().Be(model.Name);
            response.ParentDirectoryId.Should().Be(model.ParentDirectoryId);
        }

        [Fact]
        public void DocumentDomainList_ToResponse()
        {
            // Arrange
            var model = _fixture.CreateMany<DocumentDomain>(3);

            // Act
            var response = model.ToResponse();

            // Assert
            response.Documents.Should().HaveCount(3);
            response.Documents.Should().BeEquivalentTo(model.Select(x => x.ToResponse()));
        }

        [Fact]
        public void DocumentDomain_ToResponse()
        {
            // Arrange
            var model = _fixture.Create<DocumentDomain>();

            // Act
            var response = model.ToResponse();

            // Assert
            response.Id.Should().Be(model.Id);
            response.DirectoryId.Should().Be(model.DirectoryId);
            response.FileSize.Should().Be(model.FileSize);
            response.Name.Should().Be(model.Name);
            response.S3Location.Should().Be(model.S3Location);
        }
    }
}
