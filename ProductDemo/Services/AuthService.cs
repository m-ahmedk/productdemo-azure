using ProductDemo.DTOs.Auth;
using ProductDemo.Exceptions;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services.Interfaces;
using System.Net;

namespace ProductDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenService _tokenService;

        public AuthService(IUserRepository userRepository, IAuthTokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AppUser> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new AppException("Email already exists");

            var (hash, salt) = HashHelper.HashPassword(dto.Password);

            var user = new AppUser
            {
                Email = dto.Email.ToLowerInvariant(),
                PasswordHash = hash,
                PasswordStamp = salt
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) throw new AppException("Invalid credentials", (int)HttpStatusCode.Unauthorized);

            var isValid = HashHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordStamp);
            if (!isValid) throw new AppException("Invalid credentials", (int)HttpStatusCode.Unauthorized);

            return _tokenService.CreateToken(user);
        }
    }
}
