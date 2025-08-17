using AutoMapper;
using FluentAssertions;
using ProductDemo.DTOs.Product;
using ProductDemo.Models;

namespace ProductDemo.Tests.Mappings
{
    public class ProductMappingTests
    {
        private readonly IMapper _mapper;

        public ProductMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddGlobalIgnore("CreatedAt");
                cfg.AddGlobalIgnore("LastModifiedAt");
                cfg.AddGlobalIgnore("IsDeleted");
                cfg.AddGlobalIgnore("DeletedAt");
                cfg.AddProfile<ProductProfile>();
            });

            try
            {
                config.AssertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_CreateProductDto_To_Product()
        {
            var dto = new CreateProductDto { Name = "Cola", Price = 5, Quantity = 10 };

            var product = _mapper.Map<Product>(dto);

            product.Name.Should().Be("Cola");
            product.Price.Should().Be(5);
            product.Quantity.Should().Be(10);
        }

        [Fact]
        public void Should_Map_Product_To_ProductDto()
        {
            var product = new Product { Id = 1, Name = "Juice", Price = 12, Quantity = 3 };

            var dto = _mapper.Map<ProductDto>(product);

            dto.Id.Should().Be(1);
            dto.Name.Should().Be("Juice");
            dto.Price.Should().Be(12);
            dto.Quantity.Should().Be(3);
        }

        [Fact]
        public void Should_Update_Only_NonNull_Fields_From_UpdateProductDto()
        {
            var existing = new Product { Id = 50, Name = "Tea", Price = 20, Quantity = 5 };
            var dto = new UpdateProductDto { Id = 50, Quantity = 10 }; // only Quantity set

            _mapper.Map(dto, existing);

            existing.Price.Should().Be(20);    // unchanged
            existing.Name.Should().Be("Tea");  // unchanged
            existing.Quantity.Should().Be(10); // updated
        }

        [Fact]
        public void Should_Update_Name_When_Provided()
        {
            var existing = new Product { Id = 10, Name = "Old", Price = 5, Quantity = 2 };
            var dto = new UpdateProductDto { Id = 10, Name = "New" };

            _mapper.Map(dto, existing);

            existing.Name.Should().Be("New");  // updated
            existing.Price.Should().Be(5);     // unchanged
            existing.Quantity.Should().Be(2);  // unchanged
        }

        [Fact]
        public void Should_Allow_Setting_Price_To_Zero()
        {
            var existing = new Product { Id = 77, Name = "Snack", Price = 15, Quantity = 5 };
            var dto = new UpdateProductDto { Id = 77, Price = 0 }; // explicitly set to zero

            _mapper.Map(dto, existing);

            existing.Price.Should().Be(0);     // updated to zero
            existing.Name.Should().Be("Snack");// unchanged
            existing.Quantity.Should().Be(5);  // unchanged
        }

        [Fact]
        public void Should_NotOverwrite_SystemFields_When_Mapping()
        {
            var createdAt = DateTime.UtcNow.AddDays(-5);
            var modifiedAt = DateTime.UtcNow.AddDays(-1);

            var existing = new Product
            {
                Id = 1,
                Name = "Test",
                Price = 10,
                Quantity = 2,
                CreatedAt = createdAt,
                LastModifiedAt = modifiedAt,
                IsDeleted = false,
                DeletedAt = null
            };

            var dto = new UpdateProductDto { Id = 1, Quantity = 20 };

            _mapper.Map(dto, existing);

            // System fields unchanged
            existing.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromSeconds(1));
            existing.LastModifiedAt.Should().BeCloseTo(modifiedAt, TimeSpan.FromSeconds(1));
            existing.IsDeleted.Should().BeFalse();
            existing.DeletedAt.Should().BeNull();

            // Business field updated
            existing.Quantity.Should().Be(20);
        }
    }
}