using DocumentService.Encryption;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace DocumentService.TestsEncryption
{
    public class PasswordHasherTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void Check_WhenPasswordIsInvalid_ReturnsFalse()
        {
            // Arrange
            var realPassword = _fixture.Create<string>();
            var hash = _passwordHasher.Hash(realPassword);

            var password = _fixture.Create<string>();

            // Act
            var result = _passwordHasher.Check(hash, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Check_WhenPasswordIsIValid_ReturnsTrue()
        {
            // Arrange
            var realPassword = _fixture.Create<string>();
            var hash = _passwordHasher.Hash(realPassword);

            // Act
            var result = _passwordHasher.Check(hash, realPassword);

            // Assert
            result.Should().BeTrue();
        }
    }
}