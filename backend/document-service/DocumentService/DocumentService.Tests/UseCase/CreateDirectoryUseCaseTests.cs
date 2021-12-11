using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.UseCase.Interfaces;
using Moq;
using System;
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
            _mockDirectoryGateway.Verify(x => x.CreateDirectory(It.IsAny<DirectoryDomain>()));
        }
    }
}
