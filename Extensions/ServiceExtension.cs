using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using ProductDemo.DTOs.Product;
using ProductDemo.Mappings;
using ProductDemo.Models;
using ProductDemo.Repositories;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;
using ProductDemo.Services.Interfaces;
using ProductDemo.Validators.Product;

namespace ProductDemo.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddProjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddProjectValidators(this IServiceCollection services) {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
            services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();

            return services;
        }

        public static IServiceCollection AddProjectMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}