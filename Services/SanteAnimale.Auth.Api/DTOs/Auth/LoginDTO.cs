using System.ComponentModel.DataAnnotations;

namespace SanteAnimale.Auth.Api.DTOs.Auth
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
