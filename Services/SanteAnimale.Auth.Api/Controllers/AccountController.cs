using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanteAnimale.Auth.Api.DTOs.Account;
using SanteAnimale.Auth.Api.Services.Interfaces;

namespace SanteAnimale.Auth.Api.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            _logger.LogInformation("Tentative de modification du mot de passe pour l'utilisateur {UserId}.", changePasswordDto.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : données invalides.", changePasswordDto.UserId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.ChangePasswordAsync(changePasswordDto);
                return Ok("Mot de passe modifié avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la modification du mot de passe pour l'utilisateur {UserId}.", changePasswordDto.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDTO changeEmailDto)
        {
            _logger.LogInformation("Tentative de modification de l'adresse e-mail pour l'utilisateur {UserId}.", changeEmailDto.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : données invalides.", changeEmailDto.UserId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.ChangeEmailAsync(changeEmailDto);
                return Ok("Adresse e-mail modifiée avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la modification de l'adresse e-mail pour l'utilisateur {UserId}.", changeEmailDto.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount(DeleteAccountDTO deleteAccountDto)
        {
            _logger.LogInformation("Tentative de suppression du compte pour l'utilisateur {UserId}.", deleteAccountDto.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la suppression du compte pour l'utilisateur {UserId} : données invalides.", deleteAccountDto.UserId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.DeleteAccountAsync(deleteAccountDto);
                return Ok("Compte supprimé avec succès");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la suppression du compte pour l'utilisateur {UserId}.", deleteAccountDto.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }
    }
}
