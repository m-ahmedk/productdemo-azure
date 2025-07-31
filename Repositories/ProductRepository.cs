using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;

namespace ProductDemo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            // Soft delete logic – let AppDbContext handle it (already done in SaveChangesAsync)
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var exists = await _context.Products.AnyAsync(p => p.Id == product.Id);
            if (!exists) return false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }
    }
}
