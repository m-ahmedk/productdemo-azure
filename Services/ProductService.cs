using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services.Interfaces;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> AddAsync(Product product)
    {
        var exists = await _repository.ExistsByNameAsync(product.Name);
        if (exists)
            throw new InvalidOperationException("Product name must be unique");

        await _repository.AddAsync(product);
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid Product ID");

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid Product ID");

        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        if (product == null || product.Id <= 0)
            throw new ArgumentException("Invalid product");

        return await _repository.UpdateAsync(product);
    }
}