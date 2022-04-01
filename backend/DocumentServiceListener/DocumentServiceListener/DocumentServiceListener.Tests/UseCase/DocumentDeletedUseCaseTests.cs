using AutoFixture;
using AWSServerless1;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentServiceListener.Tests.UseCase
{
    public class DocumentDeletedUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IStorageUsageCache> _mockStorageUsageCache;
        private readonly DocumentDeletedUseCase _classUnderTest;

        public DocumentDeletedUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockStorageUsageCache = new Mock<IStorageUsageCache>();

            _classUnderTest = new DocumentDeletedUseCase(_mockS3Gateway.Object, _mockStorageUsageCache.Object);
        }

        [Fact]
        public async Task WhenMissingBodyParameters_ThrowsArgumentNullException()
        {
            // Arrange
            var entity = _fixture.Create<CloudStoreSnsEvent>();

            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task WhenCalled_CallsS3Gateway()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            var expectedKey = $"{entity.User.Id}/{documentId}";

            _mockS3Gateway
                .Verify(x => x.DeleteThumbnail(expectedKey), Times.Once);
        }

        [Fact]
        public async Task WhenCalled_DeletesCache()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            _mockStorageUsageCache
                .Verify(x => x.DeleteCache(It.IsAny<User>()), Times.Once);
        }
    }
}
