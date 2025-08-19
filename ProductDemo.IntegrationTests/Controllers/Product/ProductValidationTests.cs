using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProductDemo.DTOs.Product;
using ProductDemo.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests.Controllers.Product;

public class ProductValidationTests : TestBase
{
    public ProductValidationTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_Should_Return_400_When_Name_Is_Empty()
    {
        await AuthorizeAsync("Admin");

        var dto = new CreateProductDto { Name = "", Price = 10m, Quantity = 5 };

        var response = await Client.PostAsJsonAsync("/api/product", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task CreateProduct_Should_Return_400_When_Price_Is_Invalid()
    {
        await AuthorizeAsync("Admin");

        var dto = new CreateProductDto { Name = "Invalid", Price = 0, Quantity = 5 };

        var response = await Client.PostAsJsonAsync("/api/product", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Price");
    }

    [Fact]
    public async Task UpdateProduct_Should_Return_400_When_Quantity_Is_Negative()
    {
        await AuthorizeAsync("Admin");

        // create valid product first
        var createDto = new CreateProductDto { Name = "Desk", Price = 50m, Quantity = 2 };
        var createResponse = await Client.PostAsJsonAsync("/api/product", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

        var updateDto = new UpdateProductDto { Id = created!.Data!.Id, Quantity = -10 };

        var response = await Client.PatchAsJsonAsync("/api/product", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Quantity");
    }
}
