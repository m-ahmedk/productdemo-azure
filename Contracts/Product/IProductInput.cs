namespace ProductDemo.Contracts.Product
{
    // Including common properties between Dtos
    public interface IProductInput
    {
        string Name { get; set; }
        decimal Price { get; set; }
        decimal Quantity { get; set; }
    }
}