using AutoMapper;
using ProductDemo.DTOs.Product;
using ProductDemo.Models;

namespace ProductDemo.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // DTO to Entity 
            CreateMap<CreateProductDto, Product>();

            // To update non null fields only
            CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Price, opt =>
                opt.Condition((src, dest, srcMember) => src.Price.HasValue))
            .ForMember(dest => dest.Quantity, opt =>
                opt.Condition((src, dest, srcMember) => src.Quantity.HasValue))
            .ForMember(dest => dest.Name, opt =>
                opt.Condition((src, dest, srcMember) => !string.IsNullOrWhiteSpace(src.Name)));

            // Entity to DTO
            CreateMap<Product, BaseProductDto>();
            CreateMap<Product, ProductDto>();
        }
    }
}
