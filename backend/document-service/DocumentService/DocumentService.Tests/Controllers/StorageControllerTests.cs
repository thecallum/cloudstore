using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Controllers;
using DocumentService.Domain;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Tests.Helpers;
using DocumentService.UseCase;
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
        private readonly Mock<IStorageUsageUseCase> _mockStorageUsageUseCase;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        private readonly User _user;

        public StorageControllerTests()
        {
            _mockStorageUsageUseCase = new Mock<IStorageUsageUseCase>();

            _storageController = new StorageController(_mockStorageUsageUseCase.Object);

            _user = ContextHelper.CreateUser();

            _storageController.ControllerContext = new ControllerContext();
            _storageController.ControllerContext.HttpContext = new DefaultHttpContext();
            _storageController.ControllerContext.HttpContext.Items["user"] = _user;
        }

        [Fact]
        public async Task GetStorageUsage_WhenCalled_CallsUseCase()
        {
            // Arrange
            var useCaseResponse = _fixture.Create<long>();

            _mockStorageUsageUseCase
                .Setup(x => x.GetUsage(It.IsAny<User>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _storageController.GetStorageUsage();

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeOfType(typeof(StorageUsageResponse));
            ((response as OkObjectResult).Value as StorageUsageResponse).Capacity.Should().Be(_user.StorageCapacity);
            ((response as OkObjectResult).Value as StorageUsageResponse).StorageUsage.Should().Be(useCaseResponse);
        }
    }
}
