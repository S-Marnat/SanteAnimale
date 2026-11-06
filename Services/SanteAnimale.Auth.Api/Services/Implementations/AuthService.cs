using Microsoft.AspNetCore.Identity;
using SanteAnimale.Auth.Api.DTOs.Auth;
using SanteAnimale.Auth.Api.DTOs.JWT;
using SanteAnimale.Auth.Api.Entities;
using SanteAnimale.Auth.Api.Services.Interfaces;

namespace SanteAnimale.Auth.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRefreshTokenService refreshTokenService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                _logger.LogInformation("Aucun utilisateur trouvé avec l'adresse e-mail {Email}.",
                    forgotPasswordDto.Email);
                return; // Ne pas révéler que l'utilisateur n'existe pas pour des raisons de sécurité
            }

            // Générer un token de réinitialisation du mot de passe
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: remplacer par l'envoi d'un e-mail contenant le token de réinitialisation du mot de passe.
            _logger.LogInformation("Token de réinitialisation du mot de passe généré pour l'utilisateur {Email}: {Token}",
                user.Email, token);
        }

        public async Task<TokenResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Echec de la connexion pour l'utilisateur {Email} : email non trouvé.",
                    loginDto.Email);
                throw new UnauthorizedAccessException("Identifiants incorrects");
            }

            // Vérifier si le mot de passe correspond
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Echec de la connexion pour l'utilisateur {Email} : mot de passe incorrect.",
                    loginDto.Email);
                throw new UnauthorizedAccessException("Identifiants incorrects");
            }

            // Générer un token JWT et un refresh token
            var tokens = await _refreshTokenService.GenerateTokenAsync(user);
            _logger.LogInformation("Connexion réussie pour l'utilisateur {Email}.",
                loginDto.Email);

            return new TokenResponseDTO
            {
                Token = tokens.jwt,
                RefreshToken = tokens.refreshToken
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            // Révoquer le token de rafraîchissement
            var revoked = await _refreshTokenService.RevokeTokenAsync(refreshToken);

            if (!revoked)
            {
                _logger.LogWarning("Echec de la révocation du token de rafraîchissement.");
                throw new UnauthorizedAccessException("Token de rafraîchissement invalide");
            }

            _logger.LogInformation("Déconnexion réussie.");
        }

        public async Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenDTO refreshDto)
        {
            var result = await _refreshTokenService.RefreshTokenAsync(refreshDto.RefreshToken);

            if (result == null)
            {
                _logger.LogWarning("Echec du rafraîchissement du token : token de rafraîchissement invalide.");
                throw new UnauthorizedAccessException("Token de rafraîchissement invalide");
            }

            _logger.LogInformation("Rafraîchissement du token réussi.");

            return new TokenResponseDTO
            {
                Token = result.Value.jwt,
                RefreshToken = result.Value.refreshToken
            };
        }

        public async Task<TokenResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                _logger.LogWarning("Echec de l'inscription pour l'utilisateur {Email} : email déjà utilisé.",
                    registerDto.Email);
                throw new InvalidOperationException("Cette adresse e-mail est déjà utilisée");
            }

            // Créer un nouvel utilisateur
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var createResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (!createResult.Succeeded)
            {
                _logger.LogWarning("Echec de l'inscription pour l'utilisateur {Email} : {Erreur}.",
                    registerDto.Email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Impossible de créer l'utilisateur : " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            // Assigner le rôle "User" à l'utilisateur nouvellement créé
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Echec de l'inscription pour l'utilisateur {Email} : {Erreur}.",
                    registerDto.Email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Impossible d'assigner le rôle à l'utilisateur : " +
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Générer un token JWT et un refresh token
            var tokens = await _refreshTokenService.GenerateTokenAsync(user);
            _logger.LogInformation("Inscription réussie pour l'utilisateur {Email}.",
                registerDto.Email);

            return new TokenResponseDTO
            {
                Token = tokens.jwt,
                RefreshToken = tokens.refreshToken
            };
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Echec de la réinitialisation du mot de passe pour l'utilisateur {Email} : utilisateur non trouvé.",
                    resetPasswordDto.Email);
                throw new InvalidOperationException("Utilisateur non trouvé");
            }

            // Vérifier si les mots de passe correspondent
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
            {
                _logger.LogWarning("Echec de la réinitialisation du mot de passe pour l'utilisateur {Email} : les mots de passe ne correspondent pas.",
                    resetPasswordDto.Email);
                throw new InvalidOperationException("Les mots de passe ne correspondent pas");
            }

            // Réinitialiser le mot de passe
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Echec de la réinitialisation du mot de passe pour l'utilisateur {Email} : {Erreur}.",
                    resetPasswordDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Impossible de réinitialiser le mot de passe : " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Réinitialisation du mot de passe réussie pour l'utilisateur {Email}.",
                resetPasswordDto.Email);
        }
    }
}
