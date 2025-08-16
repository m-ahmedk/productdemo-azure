using FluentAssertions;
using ProductDemo.Helpers;
using Xunit;

namespace ProductDemo.Tests.Helpers
{
    public class HashHelperTests
    {
        [Fact]
        public void HashPassword_ShouldReturn_HashAndSalt()
        {
            // Arrange
            var password = "MySecret123!";

            // Act
            var (hash, salt) = HashHelper.HashPassword(password);

            // Assert
            hash.Should().NotBeNull();
            hash.Length.Should().BeGreaterThan(0);
            salt.Should().NotBeNull();
            salt.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
        {
            // Arrange
            var password = "MySecret123!";
            var (hash, salt) = HashHelper.HashPassword(password);

            // Act
            var result = HashHelper.VerifyPassword(password, hash, salt);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var (hash, salt) = HashHelper.HashPassword("CorrectPassword");

            // Act
            var result = HashHelper.VerifyPassword("WrongPassword", hash, salt);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HashPassword_ShouldThrow_WhenPasswordIsNullOrEmpty()
        {
            Action act = () => HashHelper.HashPassword("");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void VerifyPassword_ShouldThrow_WhenPasswordIsNullOrEmpty()
        {
            var (hash, salt) = HashHelper.HashPassword("Something");

            Action act = () => HashHelper.VerifyPassword("", hash, salt);
            act.Should().Throw<ArgumentException>();
        }
    }
}
