using ProductDemo.DTOs.Auth;
using ProductDemo.Models;

namespace ProductDemo.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AppUser> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }

}