using ProductDemo.Models;

namespace ProductDemo.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser> AddAsync(AppUser user);
    }
}
