using AutoFixture;
using DocumentService.Domain;
using DocumentService.Services;
using FluentAssertions;
using Xunit;

namespace DocumentService.Tests.Services
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
        public void ValidateToken_WhenTokenIsInvalid_ReturnsFalse()
        {
            // Arrange
            var token = _fixture.Create<string>();

            // Act
            var response = _tokenService.ValidateToken(token);

            // Assert
            response.Should().BeFalse();
        }

        [Fact]
        public void ValidateToken_WhenTokenIsValid_ReturnsTrue()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var token = _tokenService.CreateToken(user);

            // Act
            var response = _tokenService.ValidateToken(token);

            // Assert
            response.Should().BeTrue();
        }

        [Fact]
        public void DecodeToken_WhenTokenEncodedWithSecret_CanDecodePayload()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var token = _tokenService.CreateToken(user);

            // Act
            var response = _tokenService.DecodeToken(token);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(user.Id);
            response.Email.Should().Be(user.Email);
            response.FirstName.Should().Be(user.FirstName);
            response.LastName.Should().Be(user.LastName);
        }
    }
}
