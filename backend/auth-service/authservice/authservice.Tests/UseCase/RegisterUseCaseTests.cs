﻿using authservice.Boundary.Request;
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
    public class RegisterUseCaseTests
    {

        private readonly IRegisterUseCase _registerUseCase;
        private readonly Mock<IUserGateway> _mockUserGateway;
        private readonly Fixture _fixture = new Fixture();

        public RegisterUseCaseTests()
        {
            _mockUserGateway = new Mock<IUserGateway>();

            _registerUseCase = new RegisterUseCase(_mockUserGateway.Object);
        }

        [Fact]
        public async Task WhenCalled_ReturnsGuid()
        {
            // Arrange
            var mockRequest = _fixture.Create<RegisterRequestObject>();
            var mockUser = _fixture.Create<User>();
            var mockHash = _fixture.Create<string>();

            _mockUserGateway.Setup(x => x.RegisterUser(It.IsAny<User>()));

            // Act
            Func<Task<Guid>> func = async () => await _registerUseCase.Execute(mockRequest, mockHash).ConfigureAwait(false);

            // Assert
            await func.Should().NotThrowAsync<UserWithEmailAlreadyExistsException>();
        }

        [Fact]
        public async Task WhenEmailAlreadyInUse_ThrowsUserWithEmailAlreadyExistsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<RegisterRequestObject>();
            var mockHash = _fixture.Create<string>();

            var exception = new UserWithEmailAlreadyExistsException(mockRequest.Email);

              _mockUserGateway
                 .Setup(x => x.RegisterUser(It.IsAny<User>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task<Guid>> func = async () => { return await _registerUseCase.Execute(mockRequest, mockHash).ConfigureAwait(false); };

            // Assert
            await func.Should().ThrowAsync<UserWithEmailAlreadyExistsException>();
        }
    }
}