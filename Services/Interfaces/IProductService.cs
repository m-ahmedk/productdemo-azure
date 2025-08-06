using ProductDemo.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(int id); // removed nullable
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product); // no more bool
    Task DeleteAsync(int id);          // no more bool
}
