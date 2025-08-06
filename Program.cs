using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Extensions;
using ProductDemo.Logging;
using ProductDemo.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

LogConfigurator.ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer($"{connectionString}"));

// Register Swagger Services, Repositories, Services, Validators and Mappings via custom made Service Extension
builder.Services.AddSwaggerDocument();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProjectRepositories();
builder.Services.AddProjectServices();
builder.Services.AddProjectValidators();
builder.Services.AddProjectMappings();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add your custom middleware early in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use Swagger middleware here, can be inside IsDevelopment if only for Development
app.UseSwagger();
app.UseSwaggerUI(); // This enables browser testing

app.UseHttpsRedirection();

// request pipeline execution
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seed data
DbInitializer.Seed(app.Services);

app.Run();