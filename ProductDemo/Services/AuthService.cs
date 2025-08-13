using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.DTOs.Auth;
using ProductDemo.Exceptions;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IAuthTokenService _tokenService;

        public AuthService(AppDbContext context, IAuthTokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AppUser> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new AppException("Email already exists");

            var (hash, salt) = HashHelper.HashPassword(dto.Password);

            var user = new AppUser
            {
                Email = dto.Email.ToLowerInvariant(),
                PasswordHash = hash,
                PasswordStamp = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLowerInvariant());
            if (user == null) throw new AppException("Invalid credentials");

            var isValid = HashHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordStamp);
            if (!isValid) throw new AppException("Invalid credentials");

            return _tokenService.CreateToken(user);
        }
    }

}
