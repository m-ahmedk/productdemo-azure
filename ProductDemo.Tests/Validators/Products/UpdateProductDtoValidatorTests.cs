using FluentValidation.TestHelper;
using ProductDemo.DTOs.Product;
using ProductDemo.Validators.Product;

namespace ProductDemo.Tests.Validators.Products
{
    public class UpdateProductDtoValidatorTests
    {
        // UpdateProductDtoValidatorTests work fine with decimal because we're not testing null

        private readonly UpdateProductDtoValidator _validator = new();

        // ID RULES
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_Id_Is_Invalid(int id)
        {
            var dto = new UpdateProductDto { Id = id };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Pass_When_Id_Is_Valid_And_No_Fields_Provided()
        {
            // PATCH semantics: allowing partial updates with only Id
            var dto = new UpdateProductDto { Id = 5 };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // NAME RULES (only when provided)

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_Have_Error_When_Name_Provided_But_EmptyOrWhitespace(string name)
        {
            var dto = new UpdateProductDto { Id = 5, Name = name };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Provided_But_TooShort()
        {
            var dto = new UpdateProductDto { Id = 5, Name = "A" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Name_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Name = "Tea" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Validate_Name_When_Not_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Name = null };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        // PRICE RULES (only when provided)

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_Price_Provided_But_NotPositive(decimal price)
        {
            var dto = new UpdateProductDto { Id = 5, Price = price };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Price_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Price = 9.99m };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Not_Validate_Price_When_Not_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Price = null };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        // QUANTITY RULES (only when provided)

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Should_Have_Error_When_Quantity_Provided_But_NotPositive(decimal qty)
        {
            var dto = new UpdateProductDto { Id = 5, Quantity = qty };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Quantity_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Quantity = 2 };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Validate_Quantity_When_Not_Provided()
        {
            var dto = new UpdateProductDto { Id = 5, Quantity = null };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }
    }
}
