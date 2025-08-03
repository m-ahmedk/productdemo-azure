using FluentValidation;
using ProductDemo.Contracts.Product;
using ProductDemo.DTOs.Product;

namespace ProductDemo.Validators.Product
{
    public class ProductDtoValidatorBase<T> : AbstractValidator<T> where T : IProductInput
    {
        public ProductDtoValidatorBase()
        {
            When(p => p.Name! != null && !string.IsNullOrWhiteSpace(p.Name), () => {
                RuleFor(p => p.Name)
                    .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Product name is required")
                    .MinimumLength(2).WithMessage("Name should be greater than one character");
            });

            When(p => p.Price.HasValue, () =>
            {
                RuleFor(p => p.Price!.Value)
                    .GreaterThan(0)
                    .WithMessage("Price must be greater than 0");
            });

            When(p => p.Quantity.HasValue, () =>
            {
                RuleFor(p => p.Quantity!.Value)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");
            });
        }
    }
}
