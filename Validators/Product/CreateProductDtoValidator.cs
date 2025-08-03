using FluentValidation;
using ProductDemo.DTOs.Product;

namespace ProductDemo.Validators.Product
{
    // Format: YourValidatorName : IValidator<YourModel>
    public class CreateProductDtoValidator : ProductDtoValidatorBase<CreateProductDto> {
        public CreateProductDtoValidator()
        {
            // force non-null here
            RuleFor(x => x.Name).NotEmpty(); // Required for Create
            RuleFor(x => x.Price).NotNull();
            RuleFor(x => x.Quantity).NotNull();
        }
    }
}