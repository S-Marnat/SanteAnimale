using Microsoft.AspNetCore.Identity;
using SanteAnimale.Auth.Api.DTOs.Account;
using SanteAnimale.Auth.Api.Entities;
using SanteAnimale.Auth.Api.Services.Interfaces;

namespace SanteAnimale.Auth.Api.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<ApplicationUser> userManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task ChangeEmailAsync(ChangeEmailDTO changeEmailDto)
        {
            // Vérifier que l'utilisateur existe
            var user = await _userManager.FindByIdAsync(changeEmailDto.UserId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : utilisateur introuvable.",
                    changeEmailDto.UserId);
                throw new InvalidOperationException("Utilisateur introuvable.");
            }

            // Vérifier que le nouvel e-mail n'est pas déjà utilisé
            var existingUser = await _userManager.FindByEmailAsync(changeEmailDto.NewEmail);
            if (existingUser != null && (existingUser.Id != changeEmailDto.UserId))
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : l'adresse e-mail est déjà utilisée.",
                    changeEmailDto.UserId);
                throw new InvalidOperationException("L'adresse e-mail est déjà utilisée.");
            }

            // Mettre à jour l'adresse e-mail de l'utilisateur
            var resultEmail = await _userManager.SetEmailAsync(user, changeEmailDto.NewEmail);

            if (!resultEmail.Succeeded)
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : {Erreur}.",
                    changeEmailDto.UserId, string.Join(", ", resultEmail.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Erreur lors de la modification de l'adresse e-mail : " +
                    string.Join(", ", resultEmail.Errors.Select(e => e.Description)));
            }

            // Mettre à jour le nom d'utilisateur de l'utilisateur pour correspondre au nouvel e-mail
            var resultUserName = await _userManager.SetUserNameAsync(user, changeEmailDto.NewEmail);

            if (!resultUserName.Succeeded)
            {
                _logger.LogWarning("Echec de la modification du nom d'utilisateur pour l'utilisateur {UserId} : {Erreur}.",
                    changeEmailDto.UserId, string.Join(", ", resultUserName.Errors.Select(e => e.Description)));

                // Rétablir l'ancien e-mail en cas d'échec de la modification du nom d'utilisateur
                var resultRevertEmail = await _userManager.SetEmailAsync(user, user.Email);

                if (!resultRevertEmail.Succeeded)
                {
                    _logger.LogWarning("Echec de la restauration de l'adresse e-mail pour l'utilisateur {UserId} après un échec de la modification du nom d'utilisateur : {Erreur}.",
                        changeEmailDto.UserId, string.Join(", ", resultRevertEmail.Errors.Select(e => e.Description)));
                }

                throw new InvalidOperationException("Erreur lors de la modification du nom d'utilisateur : " +
                    string.Join(", ", resultUserName.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Adresse e-mail modifiée avec succès pour l'utilisateur {UserId}.",
                changeEmailDto.UserId);
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto)
        {
            // Vérifier que l'utilisateur existe
            var user = await _userManager.FindByIdAsync(changePasswordDto.UserId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : utilisateur introuvable.",
                    changePasswordDto.UserId);
                throw new InvalidOperationException("Utilisateur introuvable.");
            }

            // Vérifier que l'ancien mot de passe est correct
            var passwordCheck = await _userManager.CheckPasswordAsync(user, changePasswordDto.OldPassword);

            if (!passwordCheck)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : ancien mot de passe incorrect.",
                    changePasswordDto.UserId);
                throw new InvalidOperationException("Ancien mot de passe incorrect.");
            }

            // Vérifier que les nouveaux mots de passe correspondent
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : les nouveaux mots de passe ne correspondent pas.",
                    changePasswordDto.UserId);
                throw new InvalidOperationException("Les nouveaux mots de passe ne correspondent pas.");
            }

            // Vérifier que le nouveau mot de passe est différent de l'ancien
            if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : le nouveau mot de passe est identique à l'ancien.",
                    changePasswordDto.UserId);
                throw new InvalidOperationException("Le nouveau mot de passe est identique à l'ancien.");
            }

            // Mettre à jour le mot de passe de l'utilisateur
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : {Erreur}.",
                    changePasswordDto.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Erreur lors de la modification du mot de passe : " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Mot de passe modifié avec succès pour l'utilisateur {UserId}.",
                changePasswordDto.UserId);
        }

        public async Task DeleteAccountAsync(DeleteAccountDTO deleteAccountDto)
        {
            // Vérifier que l'utilisateur existe
            var user = await _userManager.FindByIdAsync(deleteAccountDto.UserId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Echec de la suppression du compte pour l'utilisateur {UserId} : utilisateur introuvable.",
                    deleteAccountDto.UserId);
                throw new InvalidOperationException("Utilisateur introuvable.");
            }

            // TODO: Ajouter les suppressions en cascades de toutes les données associées à l'utilisateur
            // (profils, plannings, soins, préventions, surveillances, notes)
            // avant de supprimer l'utilisateur Identity lui-même.

            // Supprimer l'utilisateur
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Echec de la suppression du compte pour l'utilisateur {UserId} : {Erreur}.",
                    deleteAccountDto.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Erreur lors de la suppression du compte : " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Compte supprimé avec succès pour l'utilisateur {UserId}.",
                deleteAccountDto.UserId);
        }
    }
}
