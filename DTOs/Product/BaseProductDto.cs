using ProductDemo.Contracts.Product;

namespace ProductDemo.DTOs.Product
{
    public class BaseProductDto : IProductInput // base of create/update dto + blue print for writing
    {
        public string? Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
    }
}
