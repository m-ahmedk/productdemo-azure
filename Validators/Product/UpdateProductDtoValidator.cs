using FluentValidation;
using ProductDemo.DTOs.Product;

namespace ProductDemo.Validators.Product
{
    public class UpdateProductDtoValidator : ProductDtoValidatorBase<UpdateProductDto>
    {
        public UpdateProductDtoValidator() {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("Product Id must be greater than 0");
        }
    }
}
