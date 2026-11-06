namespace SanteAnimale.Auth.Api.DTOs.JWT
{
    public class TokenResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
