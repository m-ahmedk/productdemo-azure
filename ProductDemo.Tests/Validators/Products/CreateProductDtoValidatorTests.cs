using FluentValidation.TestHelper;
using ProductDemo.DTOs.Product;
using Xunit;

namespace ProductDemo.Tests.Validators.Products
{
    public class CreateProductDtoValidatorTests
    {
        private readonly CreateProductDtoValidator _validator;

        public CreateProductDtoValidatorTests()
        {
            _validator = new CreateProductDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var model = new CreateProductDto { Name = "", Price = 10, Quantity = 5 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new CreateProductDto { Name = "A", Price = 10, Quantity = 5 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Zero_Or_Less()
        {
            var model = new CreateProductDto { Name = "Cola", Price = 0, Quantity = 5 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Zero_Or_Less()
        {
            var model = new CreateProductDto { Name = "Cola", Price = 10, Quantity = 0 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new CreateProductDto { Name = "Cola", Price = 10, Quantity = 5 };
            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
