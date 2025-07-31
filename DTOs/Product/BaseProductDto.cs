using ProductDemo.Contracts.Product;

namespace ProductDemo.DTOs.Product
{
    public class BaseProductDto : IProductInput // added interface on 26th jul 2025
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
