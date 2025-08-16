using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.DTOs.Product;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Controllers;

//[Authorize(Roles = "Admin")]
[AllowAnonymous] // for on-going development
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductController(
        IProductService productService,
        ILogger<ProductController> logger,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _logger = logger;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET all products requested.");

        var products = await _productService.GetAllAsync();
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(productDtos));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET product by ID: {ProductId}", id);

        var product = await _productService.GetByIdAsync(id);
        var dto = _mapper.Map<ProductDto>(product);

        return Ok(ApiResponse<ProductDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        _logger.LogInformation("POST create product called with name: {ProductName}", dto.Name);

        var fail = await ValidationHelper.ValidateAndFormatAsync(_createValidator, dto);
        if (fail != null) return fail;

        var product = _mapper.Map<Product>(dto);
        var created = await _productService.AddAsync(product);
        var productDto = _mapper.Map<ProductDto>(created);

        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<ProductDto>.SuccessResponse(productDto));
    }

    [HttpPatch]
    public async Task<IActionResult> Update(UpdateProductDto dto)
    {
        _logger.LogInformation("PATCH update product called with ID: {ProductId}", dto.Id);

        var fail = await ValidationHelper.ValidateAndFormatAsync(_updateValidator, dto);
        if (fail != null) return fail;

        var product = await _productService.GetByIdAsync(dto.Id); // Throws if not found
        _mapper.Map(dto, product);

        await _productService.UpdateAsync(product);

        var updatedDto = _mapper.Map<ProductDto>(product);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(updatedDto, "Product updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE product by ID: {ProductId}", id);

        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResponse("Product deleted successfully"));
    }
}
