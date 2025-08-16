using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductDemo.IntegrationTests.Services
{
    public class AuthTokenServiceTests
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthTokenServiceTests()
        {
            // Fake config with a long-enough key (>= 32 chars for HS256)
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "this_is_a_really_long_fake_test_key_1234567890!@#$" },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            // Fake DB with EF Core InMemory provider
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTokenTestDb")
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public void CreateToken_ShouldInclude_UserId_Email_And_Roles()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordStamp = new byte[] { 4, 5, 6 }
            };

            var role = new Role { Id = 1, Name = "Admin" };
            var userRole = new UserRole { UserId = 1, RoleId = 1, Role = role, User = user };

            user.UserRoles = new List<UserRole> { userRole };

            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.UserRoles.Add(userRole);
            _context.SaveChanges();

            var service = new AuthTokenService(_config, _context);

            // Act
            var token = service.CreateToken(user);

            // Decode JWT
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Assert - check claims
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            // Assert - check issuer & audience
            jwt.Issuer.Should().Be("test_issuer");
            jwt.Audiences.Should().Contain("test_audience");
        }
    }
}
