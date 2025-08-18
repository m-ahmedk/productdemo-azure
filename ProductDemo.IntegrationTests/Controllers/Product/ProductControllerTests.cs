using FluentAssertions;
using ProductDemo.DTOs.Product;
using ProductDemo.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace ProductDemo.IntegrationTests.Controllers.Product;

public class ProductControllerTests : TestBase
{
    public ProductControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetProducts_Should_Return_Empty_List_When_None_Exist()
    {
        await AuthorizeAsync();

        var response = await Client.GetAsync("/api/product");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProduct_Should_Return_401_When_Unauthorized()
    {
        Unauthorize();

        var dto = new CreateProductDto { Name = "Test Product", Price = 19.99m, Quantity = 10 };
        var response = await Client.PostAsJsonAsync("/api/product", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_And_GetById_Should_Return_Product()
    {
        await AuthorizeAsync();

        var dto = new CreateProductDto { Name = "Laptop", Price = 1500m, Quantity = 5 };
        var createResponse = await Client.PostAsJsonAsync("/api/product", dto);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        created!.Data.Should().NotBeNull();

        var id = created.Data!.Id;

        // Act - fetch by Id
        var getResponse = await Client.GetAsync($"/api/product/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        fetched!.Data!.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task UpdateProduct_Should_Modify_Existing_Product()
    {
        await AuthorizeAsync();

        // Arrange - create product first
        var dto = new CreateProductDto { Name = "Mouse", Price = 25m, Quantity = 20 };
        var createResponse = await Client.PostAsJsonAsync("/api/product", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        var productId = created!.Data!.Id;

        // Act - update
        var updateDto = new UpdateProductDto { Id = productId, Name = "Gaming Mouse", Price = 35m, Quantity = 15 };
        var updateResponse = await Client.PatchAsJsonAsync("/api/product", updateDto);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        updated!.Data!.Name.Should().Be("Gaming Mouse");
        updated.Data!.Price.Should().Be(35m);
    }

    [Fact]
    public async Task DeleteProduct_Should_Remove_Product()
    {
        await AuthorizeAsync();

        // Arrange - create product first
        var dto = new CreateProductDto { Name = "Keyboard", Price = 100m, Quantity = 5 };
        var createResponse = await Client.PostAsJsonAsync("/api/product", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        var productId = created!.Data!.Id;

        // Act - delete
        var deleteResponse = await Client.DeleteAsync($"/api/product/{productId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify it’s gone
        var getResponse = await Client.GetAsync($"/api/product/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
