using System.ComponentModel.DataAnnotations;

namespace SanteAnimale.Auth.Api.DTOs.JWT
{
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
