using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Exceptions;

namespace ProductDemo.Services;

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
            throw new AppException("Product name must be unique");

        await _repository.AddAsync(product);
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new AppException("Invalid Product ID");

        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            throw new AppException($"Product with ID {id} not found");
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new AppException("Invalid Product ID");

        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new AppException($"Product with ID {id} not found");

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        if (product == null || product.Id <= 0)
            throw new AppException("Invalid product");

        var updated = await _repository.UpdateAsync(product);
        if (!updated)
            throw new AppException("Update failed. Try again later.");
    }
}
