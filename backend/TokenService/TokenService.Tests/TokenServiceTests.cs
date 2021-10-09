using AutoFixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;
using Xunit;

namespace TokenService.Tests
{
    public class TokenServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly string _secret;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _secret = _fixture.Create<string>();
            _tokenService = new TokenService(_secret);
        }

        [Fact]
        public void ValidateToken_WhenTokenIsInvalid_ReturnsNull()
        {
            // Arrange
            var token = _fixture.Create<string>();

            // Act
            var response = _tokenService.ValidateToken(token);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void ValidateToken_WhenTokenIsValid_ReturnsPayload()
        {
            // Arrange
            var payload = _fixture.Create<Payload>();
            var token = _tokenService.CreateToken(payload);

            // Act
            var response = _tokenService.ValidateToken(token);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(payload.Id);
            response.Email.Should().Be(payload.Email);
            response.FirstName.Should().Be(payload.FirstName);
            response.LastName.Should().Be(payload.LastName);
        }

        [Fact]
        public void DecodeToken_WhenTokenEncodedWithSecret_CanDecodePayload()
        {
            // Arrange
            var payload = _fixture.Create<Payload>();
            var token = _tokenService.CreateToken(payload);

            // Act
            var response = _tokenService.DecodeToken(token);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(payload.Id);
            response.Email.Should().Be(payload.Email);
            response.FirstName.Should().Be(payload.FirstName);
            response.LastName.Should().Be(payload.LastName);

        }
    }
}
