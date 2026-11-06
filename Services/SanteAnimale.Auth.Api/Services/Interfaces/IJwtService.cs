using SanteAnimale.Auth.Api.Entities;

namespace SanteAnimale.Auth.Api.Services.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
