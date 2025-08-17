using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductDemo.Data;

namespace ProductDemo.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // 1. Remove old DbContext registrations if any
                var dbContextOptions = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();
                foreach (var d in dbContextOptions)
                    services.Remove(d);

                var dbContext = services
                    .SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));
                if (dbContext != null)
                    services.Remove(dbContext);

                // 2. Add InMemory DbContext
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // 3. Ensure DB is created + seed roles/users
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated(); // Make sure the database exists
                DbInitializer.Seed(scope.ServiceProvider).Wait();
            });
        }
    }
}