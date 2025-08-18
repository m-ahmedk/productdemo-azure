using FluentAssertions;
using Moq;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;

namespace ProductDemo.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly ProductService _sut; // system under test

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _sut = new ProductService(_repoMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowInvalidOperation_WhenNameExists()
    {
        // Arrange
        var product = new Product { Name = "Cola" };
        _repoMock.Setup(r => r.ExistsByNameAsync("Cola")).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _sut.AddAsync(product);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Product name must be unique.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdInvalid()
    {
        Func<Task> act = async () => await _sut.DeleteAsync(0);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid product ID.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFound_WhenProductDoesNotExist()
    {
        _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        Func<Task> act = async () => await _sut.DeleteAsync(99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product with ID 99 not found.");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdInvalid()
    {
        Func<Task> act = async () => await _sut.GetByIdAsync(0);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid product ID.");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFound_WhenProductMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Product?)null);

        Func<Task> act = async () => await _sut.GetByIdAsync(42);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product with ID 42 not found.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenInvalidProduct()
    {
        Func<Task> act = async () => await _sut.UpdateAsync(new Product { Id = 0 });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid product.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidOperation_WhenUpdateFails()
    {
        var product = new Product { Id = 1, Name = "Cola" };
        _repoMock.Setup(r => r.UpdateAsync(product)).ReturnsAsync(false);

        Func<Task> act = async () => await _sut.UpdateAsync(product);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Update failed. Try again later.");
    }
}
