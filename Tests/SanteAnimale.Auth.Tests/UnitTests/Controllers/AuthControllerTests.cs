using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SanteAnimale.Auth.Api.Controllers;
using SanteAnimale.Auth.Api.DTOs.Auth;
using SanteAnimale.Auth.Api.DTOs.JWT;
using SanteAnimale.Auth.Api.Services.Interfaces;

namespace SanteAnimale.Auth.Tests.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }


        // -- Register --
        [Fact]
        public async Task RegisterAsync_DonneesValides_RetourneOk()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockAuthService.Setup(s => s.RegisterAsync(registerDto))
                .ReturnsAsync(new TokenResponseDTO
                {
                    Token = "example-jwt-token",
                    RefreshToken = "example-refresh-token"
                });

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            _mockAuthService.Verify(s => s.RegisterAsync(registerDto), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var tokenResponse = Assert.IsType<TokenResponseDTO>(okResult.Value);
            Assert.Equal("example-jwt-token", tokenResponse.Token);
            Assert.Equal("example-refresh-token", tokenResponse.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange
            _authController.ModelState.AddModelError("Email", "L'adresse e-mail est requise.");

            // Act
            var result = await _authController.RegisterAsync(new RegisterDTO());

            // Assert
            _mockAuthService.Verify(s => s.RegisterAsync(It.IsAny<RegisterDTO>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task RegisterAsync_ServiceLanceInvalidOperationException_RetourneBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockAuthService.Setup(s => s.RegisterAsync(registerDto))
                .ThrowsAsync(new InvalidOperationException("Cette adresse e-mail est déjà utilisée."));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            _mockAuthService.Verify(s => s.RegisterAsync(registerDto), Times.Once);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cette adresse e-mail est déjà utilisée.", badRequestResult.Value);
        }

        [Fact]
        public async Task RegisterAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockAuthService.Setup(s => s.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            _mockAuthService.Verify(s => s.RegisterAsync(registerDto), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Login --
        [Fact]
        public async Task LoginAsync_DonneesValides_RetourneOk()
        {
            // Arrange         
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _mockAuthService.Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync(new TokenResponseDTO
                {
                    Token = "example-jwt-token",
                    RefreshToken = "example-refresh-token"
                });

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            _mockAuthService.Verify(s => s.LoginAsync(loginDto), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var tokenResponse = Assert.IsType<TokenResponseDTO>(okResult.Value);
            Assert.Equal("example-jwt-token", tokenResponse.Token);
            Assert.Equal("example-refresh-token", tokenResponse.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange         
            _authController.ModelState.AddModelError("Email", "L'adresse e-mail est requise.");

            // Act
            var result = await _authController.LoginAsync(new LoginDTO());

            // Assert
            _mockAuthService.Verify(s => s.LoginAsync(It.IsAny<LoginDTO>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task LoginAsync_ServiceLanceUnauthorizedAccessException_RetourneUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            _mockAuthService.Setup(s => s.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Identifiants incorrects."));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            _mockAuthService.Verify(s => s.LoginAsync(loginDto), Times.Once);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Identifiants incorrects.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task LoginAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            _mockAuthService.Setup(s => s.LoginAsync(loginDto))
                 .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            _mockAuthService.Verify(s => s.LoginAsync(loginDto), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Logout --
        [Fact]
        public async Task LogoutAsync_DonneesValides_RetourneOk()
        {
            // Arrange         
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.LogoutAsync(refreshTokenDto.RefreshToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authController.LogoutAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.LogoutAsync(refreshTokenDto.RefreshToken), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Déconnexion réussie.", okResult.Value);
        }

        [Fact]
        public async Task LogoutAsync_ServiceLanceUnauthorizedAccessException_RetourneUnauthorized()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.LogoutAsync(refreshTokenDto.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Token de rafraîchissement invalide."));

            // Act
            var result = await _authController.LogoutAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.LogoutAsync(refreshTokenDto.RefreshToken), Times.Once);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Token de rafraîchissement invalide.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task LogoutAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.LogoutAsync(refreshTokenDto.RefreshToken))
                 .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.LogoutAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.LogoutAsync(refreshTokenDto.RefreshToken), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Forgot Password --
        [Fact]
        public async Task ForgotPasswordAsync_DonneesValides_RetourneOk()
        {
            // Arrange         
            var forgotPasswordDto = new ForgotPasswordDTO
            {
                Email = "test@example.com"
            };

            _mockAuthService.Setup(s => s.ForgotPasswordAsync(forgotPasswordDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authController.ForgotPasswordAsync(forgotPasswordDto);

            // Assert
            _mockAuthService.Verify(s => s.ForgotPasswordAsync(forgotPasswordDto), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Demande de réinitialisation du mot de passe envoyée avec succès.", okResult.Value);
        }

        [Fact]
        public async Task ForgotPasswordAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange         
            _authController.ModelState.AddModelError("Email", "L'adresse e-mail est requise.");

            // Act
            var result = await _authController.ForgotPasswordAsync(new ForgotPasswordDTO());

            // Assert
            _mockAuthService.Verify(s => s.ForgotPasswordAsync(It.IsAny<ForgotPasswordDTO>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange         
            var forgotPasswordDto = new ForgotPasswordDTO
            {
                Email = "test@example.com"
            };

            _mockAuthService.Setup(s => s.ForgotPasswordAsync(forgotPasswordDto))
                .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.ForgotPasswordAsync(forgotPasswordDto);

            // Assert
            _mockAuthService.Verify(s => s.ForgotPasswordAsync(forgotPasswordDto), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Reset Password --
        [Fact]
        public async Task ResetPasswordAsync_DonneesValides_RetourneOk()
        {
            // Arrange         
            var resetPasswordDto = new ResetPasswordDTO
            {
                Email = "test@example.com",
                Token = "example-reset-token",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAuthService.Setup(s => s.ResetPasswordAsync(resetPasswordDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authController.ResetPasswordAsync(resetPasswordDto);

            // Assert
            _mockAuthService.Verify(s => s.ResetPasswordAsync(resetPasswordDto), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Mot de passe réinitialisé avec succès.", okResult.Value);
        }

        [Fact]
        public async Task ResetPasswordAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange         
            _authController.ModelState.AddModelError("Email", "L'adresse e-mail est requise.");

            // Act
            var result = await _authController.ResetPasswordAsync(new ResetPasswordDTO());

            // Assert
            _mockAuthService.Verify(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordDTO>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task ResetPasswordAsync_ServiceLanceInvalidOperationException_RetourneBadRequest()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDTO
            {
                Email = "test@example.com",
                Token = "example-reset-token",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAuthService.Setup(s => s.ResetPasswordAsync(resetPasswordDto))
                .ThrowsAsync(new InvalidOperationException("Utilisateur non trouvé."));

            // Act
            var result = await _authController.ResetPasswordAsync(resetPasswordDto);

            // Assert
            _mockAuthService.Verify(s => s.ResetPasswordAsync(resetPasswordDto), Times.Once);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Utilisateur non trouvé.", badRequestResult.Value);
        }

        [Fact]
        public async Task ResetPasswordAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange         
            var resetPasswordDto = new ResetPasswordDTO
            {
                Email = "test@example.com",
                Token = "example-reset-token",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAuthService.Setup(s => s.ResetPasswordAsync(resetPasswordDto))
                .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.ResetPasswordAsync(resetPasswordDto);

            // Assert
            _mockAuthService.Verify(s => s.ResetPasswordAsync(resetPasswordDto), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Refresh Token --
        [Fact]
        public async Task RefreshTokenAsync_DonneesValides_RetourneOk()
        {
            // Arrange         
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.RefreshTokenAsync(refreshTokenDto))
                .ReturnsAsync(new TokenResponseDTO
                {
                    Token = "example-jwt-token",
                    RefreshToken = "example-refresh-token-2"
                });

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.RefreshTokenAsync(refreshTokenDto), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var tokenResponse = Assert.IsType<TokenResponseDTO>(okResult.Value);
            Assert.Equal("example-jwt-token", tokenResponse.Token);
            Assert.Equal("example-refresh-token-2", tokenResponse.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ServiceLanceUnauthorizedAccessException_RetourneUnauthorized()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.RefreshTokenAsync(refreshTokenDto))
                .ThrowsAsync(new UnauthorizedAccessException("Token de rafraîchissement invalide."));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.RefreshTokenAsync(refreshTokenDto), Times.Once);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Token de rafraîchissement invalide.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task RefreshTokenAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange         
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "example-refresh-token"
            };

            _mockAuthService.Setup(s => s.RefreshTokenAsync(refreshTokenDto))
                .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(s => s.RefreshTokenAsync(refreshTokenDto), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }
    }
}
