using SanteAnimale.Auth.Api.DTOs.Auth;
using SanteAnimale.Auth.Api.DTOs.JWT;

namespace SanteAnimale.Auth.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<TokenResponseDTO?> LoginAsync(LoginDTO loginDto);
        Task LogoutAsync(string refreshToken);
        Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
        Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenDTO refreshDto);
    }
}
