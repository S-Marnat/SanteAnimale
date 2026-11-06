using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanteAnimale.Auth.Api.DTOs.Auth;
using SanteAnimale.Auth.Api.DTOs.JWT;
using SanteAnimale.Auth.Api.Services.Interfaces;

namespace SanteAnimale.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            _logger.LogInformation("Tentative d'inscription pour l'utilisateur {Email}.", registerDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de l'inscription pour l'utilisateur {Email} : données invalides.", registerDto.Email);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de l'inscription pour l'utilisateur {Email}.", registerDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            _logger.LogInformation("Tentative de connexion pour l'utilisateur {Email}.", loginDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la connexion pour l'utilisateur {Email} : données invalides.", loginDto.Email);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la connexion pour l'utilisateur {Email}.", loginDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenDTO refreshTokenDto)
        {
            _logger.LogInformation("Tentative de déconnexion.");

            try
            {
                await _authService.LogoutAsync(refreshTokenDto.RefreshToken);
                return Ok("Déconnexion réussie.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la déconnexion.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            _logger.LogInformation("Demande de réinitialisation du mot de passe pour l'utilisateur {Email}", forgotPasswordDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la demande de réinitialisation du mot de passe pour l'utilisateur {Email} : données invalides.", forgotPasswordDto.Email);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _authService.ForgotPasswordAsync(forgotPasswordDto);
                return Ok("Demande de réinitialisation du mot de passe envoyée avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la demande de réinitialisation du mot de passe pour l'utilisateur {Email}.", forgotPasswordDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            _logger.LogInformation("Tentative de réinitialisation du mot de passe pour l'utilisateur {Email}.", resetPasswordDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Echec de la réinitialisation du mot de passe pour l'utilisateur {Email} : données invalides.", resetPasswordDto.Email);
                return BadRequest("Les données fournies sont invalides.");
            }

            try
            {
                await _authService.ResetPasswordAsync(resetPasswordDto);
                return Ok("Mot de passe réinitialisé avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la réinitialisation du mot de passe pour l'utilisateur {Email}.", resetPasswordDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO refreshTokenDto)
        {
            _logger.LogInformation("Tentative de rafraîchissement du token.");

            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors du rafraîchissement du token.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }
    }
}
