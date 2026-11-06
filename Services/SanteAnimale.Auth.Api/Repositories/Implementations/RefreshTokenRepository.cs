using SanteAnimale.Auth.Api.Entities;
using SanteAnimale.Auth.Api.Repositories.Interfaces;
using SanteAnimale.Auth.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace SanteAnimale.Auth.Api.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }

        public async Task RevokeAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
