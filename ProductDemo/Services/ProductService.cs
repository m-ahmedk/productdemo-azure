using Microsoft.ApplicationInsights;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;

namespace ProductDemo.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly TelemetryClient _telemetry;

    public ProductService(IProductRepository repository, TelemetryClient telemetry)
    {
        _repository = repository;
        _telemetry = telemetry;
    }

    public async Task<Product> AddAsync(Product product)
    {
        var exists = await _repository.ExistsByNameAsync(product.Name);

        if (exists)
            throw new InvalidOperationException("Product name must be unique.");

        await _repository.AddAsync(product);

        // Track custom metric to show in AI
        _telemetry.GetMetric("ProductsCreated").TrackValue(1);

        return product;
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid product ID.");

        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
            throw new KeyNotFoundException($"Product with ID {id} not found.");
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid product ID.");

        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        if (product == null || product.Id <= 0)
            throw new ArgumentException("Invalid product.");

        var updated = await _repository.UpdateAsync(product);
     
        if (!updated)
            throw new InvalidOperationException("Update failed. Try again later.");
    }
}