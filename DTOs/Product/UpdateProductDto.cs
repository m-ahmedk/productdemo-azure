using ProductDemo.Contracts.Product;

namespace ProductDemo.DTOs.Product
{
    public class UpdateProductDto : BaseProductDto
    {
        public int Id { get; set; }
    }
}
