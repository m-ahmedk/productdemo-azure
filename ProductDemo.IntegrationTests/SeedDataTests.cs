using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProductDemo.Data;
using Xunit;

namespace ProductDemo.IntegrationTests
{
    // IClassFixture<T> -> xUnit feature => shared instance of T once, and reuse it for all test methods
    public class SeedDataTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public SeedDataTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Should_Have_AdminRole_Seeded()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Roles.Any(r => r.Name == "Admin").Should().BeTrue();
        }

        [Fact]
        public void Should_Have_DefaultUser_Seeded()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = db.Users.FirstOrDefault(u => u.Email == "mahmedvilla@gmail.com");
            Console.WriteLine($"[DEBUG] Hash: {user?.PasswordHash}");
            Console.WriteLine($"[DEBUG] Stamp: {Convert.ToBase64String(user?.PasswordStamp ?? Array.Empty<byte>())}");

            user.Should().NotBeNull();
            user!.PasswordHash.Should().NotBeNullOrEmpty();
            user.PasswordStamp.Should().NotBeNullOrEmpty();
        }
    }
}