using ProductDemo.Contracts.Product;

namespace ProductDemo.DTOs.Product
{
    public class UpdateProductDto : IProductInput
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
