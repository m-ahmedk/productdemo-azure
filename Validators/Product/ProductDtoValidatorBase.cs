using FluentValidation;
using ProductDemo.Contracts.Product;
using ProductDemo.DTOs.Product;

namespace ProductDemo.Validators.Product
{
    public class ProductDtoValidatorBase<T> : AbstractValidator<T> where T : IProductInput
    {
        public ProductDtoValidatorBase()
        {
            RuleFor(p => p.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Product name is required")
                .MinimumLength(2).WithMessage("Name should be greater than one character");

            RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0");

            RuleFor(p => p.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }
}
