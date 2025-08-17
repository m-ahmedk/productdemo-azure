using FluentValidation.TestHelper;
using ProductDemo.DTOs.Product;

namespace ProductDemo.Tests.Validators.Products
{
    public class UpdateProductDtoValidatorTests
    {
        private readonly UpdateProductDtoValidator _validator;

        public UpdateProductDtoValidatorTests()
        {
            _validator = new UpdateProductDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Invalid()
        {
            var model = new UpdateProductDto { Id = 0 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Id);

            model = new UpdateProductDto { Id = -1 };
            result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new UpdateProductDto { Id = 1, Name = "A" };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var model = new UpdateProductDto { Id = 1, Name = "Water" };
            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Zero_Or_Negative()
        {
            var model = new UpdateProductDto { Id = 1, Price = 0 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Price);

            model = new UpdateProductDto { Id = 1, Price = -5 };
            result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Price_Is_Positive()
        {
            var model = new UpdateProductDto { Id = 1, Price = 10 };
            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Zero_Or_Negative()
        {
            var model = new UpdateProductDto { Id = 1, Quantity = 0 };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity);

            model = new UpdateProductDto { Id = 1, Quantity = -3 };
            result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Quantity_Is_Positive()
        {
            var model = new UpdateProductDto { Id = 1, Quantity = 5 };
            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Only_Id_Is_Provided()
        {
            // Valid: Only Id required for partial update
            var model = new UpdateProductDto { Id = 1 };
            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
