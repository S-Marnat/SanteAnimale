using SanteAnimale.Auth.Api.DTOs.Account;

namespace SanteAnimale.Auth.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto, int userId);
        Task ChangeEmailAsync(ChangeEmailDTO changeEmailDto, int userId);
        Task DeleteAccountAsync(int userId);
    }
}
