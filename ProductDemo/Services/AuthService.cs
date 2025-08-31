using Microsoft.ApplicationInsights;
using ProductDemo.DTOs.Auth;
using ProductDemo.Helpers;
using ProductDemo.Models;
using ProductDemo.Repositories.Interfaces;
using ProductDemo.Services.Interfaces;

namespace ProductDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenService _tokenService;
        private readonly TelemetryClient _telemetry;

        public AuthService(IUserRepository userRepository, IAuthTokenService tokenService, TelemetryClient telemetry)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _telemetry = telemetry;
        }

        public async Task<AppUser> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException("Email already exists");

            var (hash, salt) = HashHelper.HashPassword(dto.Password);

            var user = new AppUser
            {
                Email = dto.Email.ToLowerInvariant(),
                PasswordHash = hash,
                PasswordStamp = salt
            };

            await _userRepository.AddAsync(user);
            _telemetry.GetMetric("RegistrationSuccess").TrackValue(1);

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var isValid = HashHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordStamp);
            if (!isValid)
            {
                // track failed login
                _telemetry.GetMetric("FailedLogins").TrackValue(1);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // track successful login
            _telemetry.GetMetric("SuccessfulLogins").TrackValue(1);

            return _tokenService.CreateToken(user);
        }
    }
}
