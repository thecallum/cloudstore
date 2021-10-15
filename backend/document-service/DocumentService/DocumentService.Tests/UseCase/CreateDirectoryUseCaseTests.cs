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
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class CreateDirectoryUseCaseTests
    {
        private readonly CreateDirectoryUseCase _useCase;
        private readonly Mock<IDirectoryGateway> _mockDirectoryGateway;

        private readonly Fixture _fixture = new Fixture();
        public CreateDirectoryUseCaseTests()
        {
            _mockDirectoryGateway = new Mock<IDirectoryGateway>();

            _useCase = new CreateDirectoryUseCase(_mockDirectoryGateway.Object);
        }

        [Fact]
        public async Task CreateDirectory_WhenValid_CallsGateway()
        {
            // Arrange
            var request = _fixture.Create<CreateDirectoryRequest>();
            var userId = Guid.NewGuid();

            // Act
            await _useCase.Execute(request, userId);

            // Assert
            _mockDirectoryGateway.Verify(x => x.CreateDirectory(It.IsAny<Directory>()));
        }
    }
}
