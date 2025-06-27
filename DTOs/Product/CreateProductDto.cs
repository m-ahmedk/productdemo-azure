using ProductDemo.Contracts.Product;

namespace ProductDemo.DTOs.Product
{
    public class CreateProductDto : IProductInput
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
