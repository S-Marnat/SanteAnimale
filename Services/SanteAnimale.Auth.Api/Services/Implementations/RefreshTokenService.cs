using Microsoft.AspNetCore.Identity;
using SanteAnimale.Auth.Api.Entities;
using SanteAnimale.Auth.Api.Repositories.Interfaces;
using SanteAnimale.Auth.Api.Services.Interfaces;
using System.Security.Cryptography;

namespace SanteAnimale.Auth.Api.Services.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenService(
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            UserManager<ApplicationUser> userManager)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<(string jwt, string refreshToken)> GenerateTokenAsync(ApplicationUser user)
        {
            var jwt = await _jwtService.GenerateTokenAsync(user);

            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = Convert.ToBase64String(bytes);

            var refreshToken = new RefreshToken
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            return (jwt, refreshToken.Token);
        }

        public async Task<(string jwt, string refreshToken)?> RefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.IsRevoked || token.Expiration < DateTime.UtcNow)
                return null;

            var user = await _userManager.FindByIdAsync(token.UserId.ToString());

            if (user == null)
                return null;

            await _refreshTokenRepository.RevokeAsync(token);

            return await GenerateTokenAsync(user);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null)
                return false;

            await _refreshTokenRepository.RevokeAsync(token);
            return true;
        }
    }
}
