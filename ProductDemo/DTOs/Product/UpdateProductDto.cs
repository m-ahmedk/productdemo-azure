// DTOs/Product/UpdateProductDto.cs
namespace ProductDemo.DTOs.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
    }
}