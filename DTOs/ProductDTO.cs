namespace ProductDemo.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; } // Optional on POST, used on GET/PUT
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
