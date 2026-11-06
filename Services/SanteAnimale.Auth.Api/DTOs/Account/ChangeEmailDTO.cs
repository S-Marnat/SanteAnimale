using System.ComponentModel.DataAnnotations;

namespace SanteAnimale.Auth.Api.DTOs.Account
{
    public class ChangeEmailDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Compare("NewEmail")]
        public string ConfirmNewEmail { get; set; } = string.Empty;
    }
}
