using SanteAnimale.Auth.Api.Entities;

namespace SanteAnimale.Auth.Api.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<(string jwt, string refreshToken)> GenerateTokenAsync(ApplicationUser user);
        Task<(string jwt, string refreshToken)?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
