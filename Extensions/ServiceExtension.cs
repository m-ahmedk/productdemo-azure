using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

                    options.Events = new JwtBearerEvents
                    {
                        // handle unauthorized with error message
                        OnChallenge = context =>
                        {
                            context.HandleResponse(); // prevents ASP.NET default 401 write

                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(ApiResponse<string>.FailResponse("Authentication failed."));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(ApiResponse<string>.FailResponse("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        }
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
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // turn off auto-validation, manual validation preferred
            });
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>(); // registers all validators
            //services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
            //services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();
            //services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            //services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();


            return services;
        }

        public static IServiceCollection AddProjectMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}