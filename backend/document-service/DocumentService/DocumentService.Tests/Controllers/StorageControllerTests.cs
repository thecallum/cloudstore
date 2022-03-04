using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Domain;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Tests.Helpers;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace DocumentService.Tests.Controllers
{
    public class StorageControllerTests
    {
        private readonly StorageController _storageController;
        private readonly Mock<IGetStorageUsageUseCase> _mockGetStorageUsageUseCase;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public StorageControllerTests()
        {
            _mockGetStorageUsageUseCase = new Mock<IGetStorageUsageUseCase>();

            _storageController = new StorageController(_mockGetStorageUsageUseCase.Object);

            _storageController.ControllerContext = new ControllerContext();
            _storageController.ControllerContext.HttpContext = new DefaultHttpContext();
            _storageController.ControllerContext.HttpContext.Items["user"] = ContextHelper.CreateUser();
        }

        [Fact]
        public async Task GetStorageUsage_WhenCalled_CallsUseCase()
        {
            // Arrange
            var useCaseResponse = _fixture.Create<StorageUsageResponse>();

            _mockGetStorageUsageUseCase
                .Setup(x => x.Execute(It.IsAny<User>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _storageController.GetStorageUsage();

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(StorageUsageResponse));
            ((response as OkObjectResult).Value as StorageUsageResponse).Capacity.Should().Be(useCaseResponse.Capacity);
            ((response as OkObjectResult).Value as StorageUsageResponse).StorageUsage.Should().Be(useCaseResponse.StorageUsage);
        }
    }
}
