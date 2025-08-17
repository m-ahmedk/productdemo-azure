using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProductDemo.Data;
using ProductDemo.Helpers;
using ProductDemo.Models;
using System.Net;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(); // bootstraps in-memory server
        }

        [Fact]
        public async Task Register_Should_Create_New_User()
        {
            // Arrange
            var newUser = new
            {
                Email = "newuser@test.com",
                Password = "P@ssword123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check DB directly
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Users.Any(u => u.Email == "newuser@test.com").Should().BeTrue();
        }

        [Fact]
        public async Task Login_Should_Return_Jwt_For_Valid_Credentials()
        {
            // Arrange: use seeded default user (DbInitializer created this)
            var login = new
            {
                Email = "mahmedvilla@gmail.com",
                Password = "ahmed123#"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            content.Should().NotBeNull();
            content!.Data.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_Should_Fail_For_Invalid_Password()
        {
            // Arrange
            var login = new
            {
                Email = "mahmedvilla@gmail.com",
                Password = "wrongpass!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
