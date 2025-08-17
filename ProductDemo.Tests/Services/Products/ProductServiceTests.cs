using FluentAssertions;
using Moq;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;
using ProductDemo.Exceptions;

namespace ProductDemo.Tests.Services.Products
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock = new();

        private ProductService CreateSut() => new ProductService(_repoMock.Object);

        // 1. ADD
        [Fact]
        public async Task AddAsync_ShouldThrow_WhenNameNotUnique()
        {
            var product = new Product { Name = "Cola" };
            _repoMock.Setup(r => r.ExistsByNameAsync("Cola")).ReturnsAsync(true);

            var sut = CreateSut();

            var act = async () => await sut.AddAsync(product);

            await act.Should().ThrowAsync<AppException>()
                .WithMessage("*unique*");
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepo_WhenValid()
        {
            var product = new Product { Name = "Water", Price = 5, Quantity = 3 };
            _repoMock.Setup(r => r.ExistsByNameAsync("Water")).ReturnsAsync(false);
            _repoMock.Setup(r => r.AddAsync(product)).ReturnsAsync(product);
            var sut = CreateSut();

            var created = await sut.AddAsync(product);

            created.Should().NotBeNull();
            _repoMock.Verify(r => r.AddAsync(product), Times.Once);
        }

        // 2. GET BY ID
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetByIdAsync_ShouldThrow_WhenIdInvalid(int id)
        {
            var sut = CreateSut();

            var act = async () => await sut.GetByIdAsync(id);

            await act.Should().ThrowAsync<AppException>().WithMessage("*Invalid Product ID*");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);
            var sut = CreateSut();

            var act = async () => await sut.GetByIdAsync(99);

            await act.Should().ThrowAsync<AppException>().WithMessage("*not found*");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_WhenFound()
        {
            var product = new Product { Id = 10, Name = "Tea" };
            _repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(product);
            var sut = CreateSut();

            var result = await sut.GetByIdAsync(10);

            result.Id.Should().Be(10);
            result.Name.Should().Be("Tea");
        }

        // 3. UPDATE
        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenInvalidProduct()
        {
            var sut = CreateSut();

            var act1 = async () => await sut.UpdateAsync(null!);
            var act2 = async () => await sut.UpdateAsync(new Product { Id = 0 });

            await act1.Should().ThrowAsync<AppException>();
            await act2.Should().ThrowAsync<AppException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenRepoUpdateFails()
        {
            var prod = new Product { Id = 42, Name = "X" };
            _repoMock.Setup(r => r.UpdateAsync(prod)).ReturnsAsync(false);
            var sut = CreateSut();

            var act = async () => await sut.UpdateAsync(prod);

            await act.Should().ThrowAsync<AppException>().WithMessage("*Update failed*");
        }

        [Fact]
        public async Task UpdateAsync_ShouldPass_WhenRepoUpdates()
        {
            var prod = new Product { Id = 42, Name = "X" };
            _repoMock.Setup(r => r.UpdateAsync(prod)).ReturnsAsync(true);
            var sut = CreateSut();

            await sut.UpdateAsync(prod);

            _repoMock.Verify(r => r.UpdateAsync(prod), Times.Once);
        }

        // 4. DELETE
        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task DeleteAsync_ShouldThrow_WhenIdInvalid(int id)
        {
            var sut = CreateSut();

            var act = async () => await sut.DeleteAsync(id);

            await act.Should().ThrowAsync<AppException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotFound()
        {
            _repoMock.Setup(r => r.DeleteAsync(100)).ReturnsAsync(false);
            var sut = CreateSut();

            var act = async () => await sut.DeleteAsync(100);

            await act.Should().ThrowAsync<AppException>().WithMessage("*not found*");
        }

        [Fact]
        public async Task DeleteAsync_ShouldPass_WhenDeleted()
        {
            _repoMock.Setup(r => r.DeleteAsync(5)).ReturnsAsync(true);
            var sut = CreateSut();

            await sut.DeleteAsync(5);

            _repoMock.Verify(r => r.DeleteAsync(5), Times.Once);
        }
    }
}
