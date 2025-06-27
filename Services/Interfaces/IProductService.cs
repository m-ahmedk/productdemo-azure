using ProductDemo.DTOs.Product;
using ProductDemo.Models;

namespace ProductDemo.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> AddAsync(CreateProductDto createProductDto);
        Task<bool> UpdateAsync(UpdateProductDto updateProductDto);
        Task<bool> DeleteAsync(int id);

    }
}
