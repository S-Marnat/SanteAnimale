using System.ComponentModel.DataAnnotations;

namespace SanteAnimale.Auth.Api.DTOs.Account
{
    public class DeleteAccountDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
