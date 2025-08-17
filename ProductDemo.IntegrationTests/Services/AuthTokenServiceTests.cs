using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductDemo.IntegrationTests.Services
{
    public class AuthTokenServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthTokenServiceTests(CustomWebApplicationFactory<Program> factory)
        {
            using var scope = factory.Services.CreateScope();
            _config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AuthTokenTestDb")
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public void CreateToken_ShouldInclude_UserId_Email_And_Roles()
        {
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

            var token = service.CreateToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            jwt.Issuer.Should().Be(_config["Jwt:Issuer"]);
            jwt.Audiences.Should().Contain(_config["Jwt:Audience"]);
        }
    }
}
