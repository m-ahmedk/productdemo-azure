using FluentAssertions;
using Moq;
using ProductDemo.DTOs.Auth;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Tests.Services;

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
    public async Task RegisterAsync_ShouldThrowInvalidOperation_WhenEmailExists()
    {
        var dto = new RegisterDto { Email = "test@example.com", Password = "pass" };
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("test@example.com")).ReturnsAsync(true);

        Func<Task> act = async () => await _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_ShouldLowercaseEmail_And_ReturnUser()
    {
        var dto = new RegisterDto { Email = "TEST@Example.com", Password = "pass123" };
        _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Return the same AppUser that AuthService passes in
        _userRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AppUser>()))
            .ReturnsAsync((AppUser u) => u);

        var result = await _sut.RegisterAsync(dto);

        result.Email.Should().Be("test@example.com");
        result.PasswordHash.Should().NotBeNullOrEmpty();
        result.PasswordStamp.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        var dto = new LoginDto { Email = "missing@example.com", Password = "pass" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((AppUser?)null);

        Func<Task> act = async () => await _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenPasswordInvalid()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "wrongpass" };
        var user = new AppUser
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = [1,2,3],
            PasswordStamp = [4,5,6]
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Force HashHelper to fail
        // Normally, you'd abstract HashHelper too for mocking, 
        // but here we rely on wrong password failing naturally.
        Func<Task> act = async () => await _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "correctpass" };

        // Generate a real hash for "correctpass"
        var (hash, salt) = HashHelper.HashPassword("correctpass");

        var user = new AppUser
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = hash,
            PasswordStamp = salt
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.CreateToken(user)).Returns("fake-jwt-token");

        var token = await _sut.LoginAsync(dto);

        token.Should().Be("fake-jwt-token");
    }
}
