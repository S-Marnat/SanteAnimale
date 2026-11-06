using System.ComponentModel.DataAnnotations;

namespace SanteAnimale.Auth.Api.DTOs.Auth
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
