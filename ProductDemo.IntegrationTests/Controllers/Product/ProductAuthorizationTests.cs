using FluentAssertions;
using ProductDemo.DTOs.Product;
using System.Net;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests.Controllers.Product
{
    public class ProductAuthorizationTests : TestBase
    {
        public ProductAuthorizationTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task GetProducts_Should_Return_401_When_NotAuthenticated()
        {
            Unauthorize();

            var response = await Client.GetAsync("/api/product");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateProduct_Should_Return_401_When_NotAuthenticated()
        {
            Unauthorize();

            var dto = new CreateProductDto { Name = "Unauthorized", Price = 19.99m, Quantity = 5 };
            var response = await Client.PostAsJsonAsync("/api/product", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateProduct_Should_Return_401_When_NotAuthenticated()
        {
            Unauthorize();

            var updateDto = new UpdateProductDto { Id = 1, Name = "Hacker Update" };
            var response = await Client.PatchAsJsonAsync("/api/product", updateDto);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteProduct_Should_Return_401_When_NotAuthenticated()
        {
            Unauthorize();

            var response = await Client.DeleteAsync("/api/product/1");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // Optional: if you implement role-based policies later (e.g., Admin vs User)
        [Fact]
        public async Task CreateProduct_Should_Return_403_When_AuthenticatedButNotInAdminRole()
        {
            // Arrange: authorize with a "User" role account (not "Admin")
            await AuthorizeAsync(role: "User");

            var dto = new CreateProductDto { Name = "Forbidden", Price = 19.99m, Quantity = 5 };
            var response = await Client.PostAsJsonAsync("/api/product", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
