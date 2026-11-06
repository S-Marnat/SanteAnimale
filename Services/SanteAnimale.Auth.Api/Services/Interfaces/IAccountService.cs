using SanteAnimale.Auth.Api.DTOs.Account;

namespace SanteAnimale.Auth.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto);
        Task ChangeEmailAsync(ChangeEmailDTO changeEmailDto);
        Task DeleteAccountAsync(DeleteAccountDTO deleteAccountDto);
    }
}
