using AutoMapper;
using FluentAssertions;
using ProductDemo.DTOs.Product;
using ProductDemo.Mappings;
using ProductDemo.Models;

namespace ProductDemo.Tests.Mappings
{
    public class ProductProfileTests
    {
        private readonly IMapper _mapper;

        public ProductProfileTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddGlobalIgnore("CreatedAt");
                cfg.AddGlobalIgnore("LastModifiedAt");
                cfg.AddGlobalIgnore("IsDeleted");
                cfg.AddGlobalIgnore("DeletedAt");
                cfg.AddGlobalIgnore("Id");
                cfg.AddProfile<ProductProfile>();
            });

            config.AssertConfigurationIsValid(); // ensures all maps are valid
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_CreateProductDto_To_Product()
        {
            var dto = new CreateProductDto { Name = "Test", Price = 10, Quantity = 5 };
            var entity = _mapper.Map<Product>(dto);

            entity.Name.Should().Be("Test");
            entity.Price.Should().Be(10);
            entity.Quantity.Should().Be(5);
        }

        [Fact]
        public void Should_Map_UpdateProductDto_To_Product_PartialUpdate()
        {
            var entity = new Product { Id = 1, Name = "Old", Price = 5, Quantity = 2 };

            var dto = new UpdateProductDto { Id = 1, Name = "NewName" };

            _mapper.Map(dto, entity);

            entity.Id.Should().Be(1);
            entity.Name.Should().Be("NewName"); // updated
            entity.Price.Should().Be(5);        // unchanged
            entity.Quantity.Should().Be(2);     // unchanged
        }

        [Fact]
        public void Should_Map_Product_To_ProductDto()
        {
            var product = new Product { Id = 1, Name = "Shirt", Price = 20, Quantity = 3 };
            var dto = _mapper.Map<ProductDto>(product);

            dto.Name.Should().Be("Shirt");
            dto.Price.Should().Be(20);
            dto.Quantity.Should().Be(3);
        }
    }
}

