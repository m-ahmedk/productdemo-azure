using FluentValidation.TestHelper;
using ProductDemo.DTOs.Product;
using ProductDemo.Validators.Product;

namespace ProductDemo.Tests.Validators.Products
{
    public class CreateProductDtoValidatorTests
    {
        // CreateProductDtoValidatorTests need double? because testing null

        private readonly CreateProductDtoValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            // Arrange: create DTOs with invalid names
            var nullName = new CreateProductDto { Name = null, Price = 10, Quantity = 2 };
            var emptyName = new CreateProductDto { Name = "", Price = 10, Quantity = 2 };
            var whitespaceName = new CreateProductDto { Name = "  ", Price = 10, Quantity = 2 };

            // Act & Assert
            _validator.TestValidate(nullName).ShouldHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(emptyName).ShouldHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(whitespaceName).ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Short()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "A", Price = 10, Quantity = 2 };

            // Act & Assert
            _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0.0)]
        [InlineData(-5.0)]
        public void Should_Have_Error_When_Price_Is_Invalid(double? price)
        {
            var dto = new CreateProductDto { Name = "Product", Price = (decimal?)price, Quantity = 1 };
            _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0.0)]
        [InlineData(-1.0)]
        public void Should_Have_Error_When_Quantity_Is_Invalid(double? quantity)
        {
            var dto = new CreateProductDto { Name = "Product", Price = 10, Quantity = (decimal?)quantity };
            _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var dto = new CreateProductDto { Name = "Valid Product", Price = 15, Quantity = 5 };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // Edge Case - Name with extra spaces, but valid length
        [Fact]
        public void Should_Trim_And_Pass_When_Name_Has_Extra_Spaces_But_Valid()
        {
            var dto = new CreateProductDto { Name = "  Water  ", Price = 2, Quantity = 3 };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}