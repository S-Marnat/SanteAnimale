using SanteAnimale.Auth.Api.Entities;

namespace SanteAnimale.Auth.Api.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken refreshToken);
    }
}
