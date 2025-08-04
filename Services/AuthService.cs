using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.DTOs.Auth;
using ProductDemo.Exceptions;
using ProductDemo.Models;
using ProductDemo.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

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

            using var hmac = new HMACSHA256();
            var user = new AppUser
            {
                Email = dto.Email.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordStamp = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
            if (user == null) throw new AppException("Invalid credentials");

            using var hmac = new HMACSHA256(user.PasswordStamp);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!hash.SequenceEqual(user.PasswordHash))
                throw new AppException("Invalid credentials");

            return _tokenService.CreateToken(user);
        }
    }

}
