using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SanteAnimale.Auth.Api.Entities;
using SanteAnimale.Auth.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SanteAnimale.Auth.Api.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            // Récupération des rôles de l’utilisateur
            var roles = await _userManager.GetRolesAsync(user);

            // Construction de la liste des claims (information embarquée dans le token)
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Email,
                        user.Email));
            }

            // Ajout des rôles dans les claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Création de la clé de signature
            var secret =
                _configuration["Jwt:Key"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("La clé JWT est absente.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

            // Création des credentials de signature (garantit que le token n’a pas été modifié)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Vérification de la durée du token dans la configuration
            if (!double.TryParse(
                _configuration["Jwt:DurationInMinutes"],
                out var duree))
            {
                throw new InvalidOperationException(
                    "La durée du JWT est invalide.");
            }

            // Construction du JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duree),
                signingCredentials: creds
            );

            // Conversion du token en chaîne de caractères
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
