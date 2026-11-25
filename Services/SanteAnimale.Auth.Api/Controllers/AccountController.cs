using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanteAnimale.Auth.Api.DTOs.Account;
using SanteAnimale.Auth.Api.Services.Interfaces;
using System.Security.Claims;

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
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDto)
        {
            _logger.LogInformation("Tentative de modification d'un mot de passe.");
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : utilisateur non authentifié.",
                    userId);
                return Unauthorized("Utilisateur non authentifié.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la modification du mot de passe pour l'utilisateur {UserId} : données invalides.",
                    userId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.ChangePasswordAsync(changePasswordDto, int.Parse(userId));
                return Ok("Mot de passe modifié avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la modification du mot de passe pour l'utilisateur {UserId}.",
                    userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailDTO changeEmailDto)
        {
            _logger.LogInformation("Tentative de modification d'une adresse e-mail.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : utilisateur non authentifié.",
                    userId);
                return Unauthorized("Utilisateur non authentifié.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la modification de l'adresse e-mail pour l'utilisateur {UserId} : données invalides.",
                    userId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.ChangeEmailAsync(changeEmailDto, int.Parse(userId));
                return Ok("Adresse e-mail modifiée avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la modification de l'adresse e-mail pour l'utilisateur {UserId}.",
                    userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccountAsync()
        {
            _logger.LogInformation("Tentative de suppression d'un compte utilisateur.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Echec de la suppression du compte pour l'utilisateur {UserId} : utilisateur non authentifié.",
                    userId);
                return Unauthorized("Utilisateur non authentifié.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la suppression du compte pour l'utilisateur {UserId} : données invalides.",
                    userId);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _accountService.DeleteAccountAsync(int.Parse(userId));
                return Ok("Compte supprimé avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la suppression du compte pour l'utilisateur {UserId}.",
                    userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }
    }
}
