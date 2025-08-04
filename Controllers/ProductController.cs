using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.DTOs.Product;
using ProductDemo.Models;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET all products requested.");
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET product by ID: {ProductId}", id);

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for ID: {ProductId}", id);
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            _logger.LogInformation("POST create product called with name: {ProductName}", dto.Name);

            try
            {
                var product = _mapper.Map<Product>(dto);
                var created = await _productService.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Product creation failed due to business rule violation.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                throw; // GlobalExceptionMiddleware will catch
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            _logger.LogInformation("PATCH update product called with ID: {ProductId}", dto.Id);

            try
            {
                // Get existing entity from DB
                var product = await _productService.GetByIdAsync(dto.Id);
                if (product == null)
                    return NotFound();

                // Map non-null fields only
                _mapper.Map(dto, product);

                var result = await _productService.UpdateAsync(product);
                return result ? NoContent() : StatusCode(500, "Update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating product with ID: {ProductId}", dto.Id);
                throw;
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE product by ID: {ProductId}", id);

            var result = await _productService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
