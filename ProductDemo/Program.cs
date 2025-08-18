using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Extensions;
using ProductDemo.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

LogConfigurator.ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DB Context, avoid DefaultConnection for Test
if (!builder.Environment.IsEnvironment("Test"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer($"{connectionString}"));
}

// Register Model Binders, Swagger Services, Repositories, Services, Validators and Mappings via custom made Service Extension
builder.Services.AddCustomBinders();
builder.Services.AddCustomJsonConverters();
builder.Services.AddSwaggerDocument();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProjectRepositories();
builder.Services.AddProjectServices();
builder.Services.AddProjectValidators();
builder.Services.AddProjectMappings();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // use my custom IExceptionHandler
builder.Services.AddProblemDetails(); // registers ProblemDetails (RFC 7807) formatting support
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Standard exception handler
app.UseExceptionHandler(); // Hooks the exception handling middleware

// Use Swagger middleware here, can be inside IsDevelopment if only for Development
app.UseSwagger();
app.UseSwaggerUI(); // This enables browser testing

app.UseHttpsRedirection();

// request pipeline execution
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seed data
await DbInitializer.Seed(app.Services);

app.Run();

// make Program public/accessible, for integration test project
public partial class Program { }