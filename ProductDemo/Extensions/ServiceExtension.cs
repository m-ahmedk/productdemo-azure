using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductDemo.Converters;
using ProductDemo.Helpers;
using ProductDemo.Repositories;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services;
using ProductDemo.Services.Interfaces;
using ProductDemo.Validators.Auth;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ProductDemo.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddCustomBinders(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new CleanStringModelBinderProvider());
            });

            return services;
        }

        public static IServiceCollection AddCustomJsonConverters(this IServiceCollection services)
        {
            services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new CleanStringJsonConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                        )
                    };
                });

            services.AddAuthorization(); //  DI registration

            return services;
        }

        public static IServiceCollection AddSwaggerDocument(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });

                // Add JWT Auth option
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like: Bearer {your token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddProjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddProjectValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>(); // registers all validators
            //services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
            //services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();
            //services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            //services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();


            return services;
        }

        public static IServiceCollection AddProjectMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                // globally ignore EF base entity properties, managed by system (appdbcontext)
                cfg.AddGlobalIgnore("CreatedAt");
                cfg.AddGlobalIgnore("LastModifiedAt");
                cfg.AddGlobalIgnore("IsDeleted");
                cfg.AddGlobalIgnore("DeletedAt");
            }, Assembly.GetExecutingAssembly());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}