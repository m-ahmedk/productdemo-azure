using AutoMapper;
using FluentValidation;
using ProductDemo.DTOs.Product;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddAsync(CreateProductDto createProductDto)
        {
            var exists = await _repository.ExistsByNameAsync(createProductDto.Name);
            if (exists) throw new InvalidOperationException($"Product name must be unique");

            var product = _mapper.Map<Product>(createProductDto);
            
            await _repository.AddAsync(product);
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Product ID");
            
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Product ID");
            Product? product = await _repository.GetByIdAsync(id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null) throw new ArgumentException("Product is not provided");

            if (updateProductDto.Id <= 0) throw new ArgumentException("Invalid Product ID");

            bool isUpdated = await _repository.UpdateAsync(product);

            return ;
        }
    }
}
