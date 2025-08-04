using ProductDemo.Models;

namespace ProductDemo.Services.Interfaces
{
    public interface IAuthTokenService
    {
        string CreateToken(AppUser user);
    }
}
