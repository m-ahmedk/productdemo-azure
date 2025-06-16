using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"{connectionString}"));


// Register Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use Swagger middleware here
app.UseSwagger();
app.UseSwaggerUI(); // This enables browser testing

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
