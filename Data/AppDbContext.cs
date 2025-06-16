using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;

namespace ProductDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } 
        public DbSet<AppUser> Users { get; set; }
    }
}
