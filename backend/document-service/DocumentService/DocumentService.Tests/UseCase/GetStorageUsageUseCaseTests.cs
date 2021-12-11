using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class GetStorageUsageUseCaseTests
    {
        private readonly GetStorageUsageUseCase _useCase;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;

        private readonly Fixture _fixture = new Fixture();
        public GetStorageUsageUseCaseTests()
        {
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _useCase = new GetStorageUsageUseCase(_mockDocumentGateway.Object);
        }

        [Fact]
        public async Task WhenCalled_ReturnsStorageUsage()
        {
            // Arrange
            var user = _fixture.Build<User>().With(x => x.StorageCapacity, long.MaxValue).Create();

            var gatewayResponse = _fixture.Create<StorageUsageDomain>();

            _mockDocumentGateway
                .Setup(x => x.GetUsage(It.IsAny<User>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var result = await _useCase.Execute(user);

            // Assert
            result.Capacity.Should().Be(gatewayResponse.Capacity);
            result.StorageUsage.Should().Be(gatewayResponse.StorageUsage);

        }
    }
}
