using authservice.Boundary.Request;
using authservice.Controllers;
using authservice.Domain;
using authservice.Encryption;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using authservice.JWT;
using authservice.UseCase;
using authservice.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace authservice.Tests.UseCase
{
    public class CheckUseCaseTests
    {
        private readonly ICheckUseCase _checkUseCase;
        private readonly Mock<IUserGateway> _mockUserGateway;
        private readonly Fixture _fixture = new Fixture();

        public CheckUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _checkUseCase = new CheckUseCase(_mockUserGateway.Object);
        }


        [Fact]
        public async Task WhenUserDoesntExist_ReturnsNull()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();

            _mockUserGateway.Setup(x => x.GetUserByEmailAddress(It.IsAny<string>())).ReturnsAsync((UserDb) null);

            // Act
            var result = await _checkUseCase.Execute(mockEmail).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task WhenUserExists_ReturnsUser()
        {
            // Arrange
            var mockEmail = _fixture.Create<string>();
            var mockUser = _fixture.Create<UserDb>();

            _mockUserGateway.Setup(x => x.GetUserByEmailAddress(It.IsAny<string>())).ReturnsAsync(mockUser);

            // Act
            var result = await _checkUseCase.Execute(mockEmail).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(User));
            result.Id.Should().Be(mockUser.Id);
            result.Email.Should().Be(mockUser.Email);
            result.FirstName.Should().Be(mockUser.FirstName);
            result.LastName.Should().Be(mockUser.LastName);
        }
    }
}
