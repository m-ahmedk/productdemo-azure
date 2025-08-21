using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Extensions;
using ProductDemo.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Config: appsettings -> user secrets (dev) -> env vars
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();  // load env first


if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Logging
LogConfigurator.ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

if (!builder.Environment.IsEnvironment("Test"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddCustomBinders();
builder.Services.AddCustomJsonConverters();
builder.Services.AddSwaggerDocument();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProjectRepositories();
builder.Services.AddProjectServices();
builder.Services.AddProjectValidators();
builder.Services.AddProjectMappings();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Run migrations + seed, and exclude test environment from this
if (!app.Environment.IsEnvironment("Test"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}

await DbInitializer.Seed(app.Services);

app.Run();

public partial class Program { }