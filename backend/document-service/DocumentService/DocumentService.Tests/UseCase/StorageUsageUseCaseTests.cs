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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class StorageUsageUseCaseTests
    {
        private readonly StorageUsageUseCase _useCase;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IStorageUsageCache> _mockCache;

        private readonly Fixture _fixture = new Fixture();

        public StorageUsageUseCaseTests()
        {
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockCache = new Mock<IStorageUsageCache>();

            _useCase = new StorageUsageUseCase(
                _mockDocumentGateway.Object,
                _mockCache.Object);
        }

        [Fact]
        public async Task GetUsage_WhenCalled_ReturnsStorageUsage()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();

            var gatewayResponse = _fixture.Create<StorageUsageDomain>();

            _mockDocumentGateway
                .Setup(x => x.GetUsage(It.IsAny<User>()))
                .ReturnsAsync(gatewayResponse);

            _mockCache
                .Setup(x => x.GetValue(It.IsAny<Guid>()))
                .ReturnsAsync((long?) null);

            // Act
            var result = await _useCase.GetUsage(user);

            // Assert
            result.Should().Be(gatewayResponse.StorageUsage);
        }

        [Fact]
        public async Task GetUsage_WhenValueInCache_ReadsFromCache()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();

            var cacheResponse = _fixture.Create<long>();

            _mockCache
                .Setup(x => x.GetValue(It.IsAny<Guid>()))
                .ReturnsAsync(cacheResponse);

            // Act
            var result = await _useCase.GetUsage(user);

            // Assert
            result.Should().Be(cacheResponse);

            _mockDocumentGateway
                .Verify(x => x.GetUsage(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUsage_WhenValueNotInCache_SavesValueToCache()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();

            var gatewayResponse = _fixture.Create<StorageUsageDomain>();

            _mockDocumentGateway
                .Setup(x => x.GetUsage(It.IsAny<User>()))
                .ReturnsAsync(gatewayResponse);

            _mockCache
                .Setup(x => x.GetValue(It.IsAny<Guid>()))
                .ReturnsAsync((long?)null);

            // Act
            var result = await _useCase.GetUsage(user);

            // Assert
            _mockCache
                .Verify(x => x.UpdateValue(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUsage_WhenValueNotInCache_GetsValueFromDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();
            var difference = _fixture.Create<long>();

            _mockCache
                .Setup(x => x.GetValue(It.IsAny<Guid>()))
                .ReturnsAsync((long?) null);

            var gatewayResponse = _fixture.Create<StorageUsageDomain>();

            _mockDocumentGateway
                .Setup(x => x.GetUsage(It.IsAny<User>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            await _useCase.UpdateUsage(user, difference);

            // Assert
            long expectedValue = gatewayResponse.StorageUsage + difference;

            _mockCache
                .Verify(x => x.UpdateValue(It.IsAny<Guid>(), expectedValue), Times.Once);
        }

        [Fact]
        public async Task UpdateUsage_WhenValueInCache_GetsValueFromCache()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();
            var difference = _fixture.Create<long>();

            var cacheValue = _fixture.Create<long>();

            _mockCache
                .Setup(x => x.GetValue(It.IsAny<Guid>()))
                .ReturnsAsync(cacheValue);

            // Act
            await _useCase.UpdateUsage(user, difference);

            // Assert
            long expectedValue = cacheValue + difference;

            _mockCache
                .Verify(x => x.UpdateValue(It.IsAny<Guid>(), expectedValue), Times.Once);

            _mockDocumentGateway
                .Verify(x => x.GetUsage(It.IsAny<User>()), Times.Never);
        }
    }
}
