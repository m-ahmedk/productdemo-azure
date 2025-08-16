using FluentAssertions;
using Moq;
using ProductDemo.DTOs.Auth;
using ProductDemo.Exceptions;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;
using ProductDemo.Services.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace ProductDemo.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IAuthTokenService> _tokenServiceMock;
        private readonly AuthService _sut; // System Under Test

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<IAuthTokenService>();
            _sut = new AuthService(_userRepoMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var dto = new RegisterDto { Email = "test@example.com", Password = "Password123" };
            _userRepoMock.Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ReturnsAsync(false);

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<AppUser>()))
                .ReturnsAsync((AppUser u) => u); // return the same user

            // Act
            var result = await _sut.RegisterAsync(dto);

            // Assert
            result.Email.Should().Be(dto.Email.ToLowerInvariant());
            result.PasswordHash.Should().NotBeNull();
            result.PasswordStamp.Should().NotBeNull();
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<AppUser>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var dto = new RegisterDto { Email = "existing@example.com", Password = "Password123" };
            _userRepoMock.Setup(r => r.ExistsByEmailAsync(dto.Email))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _sut.RegisterAsync(dto);

            // Assert
            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Email already exists");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@example.com", Password = "Password123" };
            var (hash, salt) = HashHelper.HashPassword(dto.Password);
            var user = new AppUser
            {
                Id = 1,
                Email = dto.Email.ToLower(),
                PasswordHash = hash,
                PasswordStamp = salt
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            _tokenServiceMock.Setup(t => t.CreateToken(user))
                .Returns("fake-jwt-token");

            // Act
            var result = await _sut.LoginAsync(dto);

            // Assert
            result.Should().Be("fake-jwt-token");
        }


        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var dto = new LoginDto { Email = "notfound@example.com", Password = "Password123" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser?)null);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@example.com", Password = "wrongpassword" };
            var (hash, salt) = HashHelper.HashPassword("correctpassword");
            var user = new AppUser { Id = 1, Email = dto.Email, PasswordHash = hash, PasswordStamp = salt };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            await act.Should().ThrowAsync<AppException>()
                .WithMessage("Invalid credentials");
        }
    }
}
