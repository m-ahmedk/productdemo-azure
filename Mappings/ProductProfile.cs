using AutoMapper;
using ProductDemo.DTOs.Product;
using ProductDemo.Models;

namespace ProductDemo.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile() {

            // DTO to Entity 
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Entity to DTO
            CreateMap<Product, BaseProductDto>();
        }
    }
}
