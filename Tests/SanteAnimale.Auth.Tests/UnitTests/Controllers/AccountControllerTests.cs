using Moq;
using SanteAnimale.Auth.Api.Controllers;
using SanteAnimale.Auth.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SanteAnimale.Auth.Api.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SanteAnimale.Auth.Tests.UnitTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly AccountController _accountController;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<ILogger<AccountController>> _mockLogger;

        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _accountController = new AccountController(_mockAccountService.Object, _mockLogger.Object);
        }


        // -- Change Password --
        [Fact]
        public async Task ChangePasswordAsync_DonneesValides_RetourneOk()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAccountService.Setup(s => s.ChangePasswordAsync(changePasswordDto, userId))
                    .Returns(Task.CompletedTask);
            
            // Act
            var result = await _accountController.ChangePasswordAsync(changePasswordDto);
            
            // Assert
            _mockAccountService.Verify(s => s.ChangePasswordAsync(changePasswordDto, userId), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Mot de passe modifié avec succès.", okResult.Value);
        }

        [Fact]
        public async Task ChangePasswordAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _accountController.ModelState.AddModelError("OldPassword", "Le mot de passe actuel est requis.");

            // Act
            var result = await _accountController.ChangePasswordAsync(new ChangePasswordDTO());

            // Assert
            _mockAccountService.Verify(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDTO>(), It.IsAny<int>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangePasswordAsync_UtilisateurNonAuthentifie_RetourneBadRequest()
        {
            // Arrange
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // Utilisateur non authentifié
                }
            };

            // Act
            var result = await _accountController.ChangePasswordAsync(new ChangePasswordDTO());

            // Assert
            _mockAccountService.Verify(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDTO>(), It.IsAny<int>()), Times.Never);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Utilisateur non authentifié.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task ChangePasswordAsync_ServiceLanceInvalidOperationException_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAccountService.Setup(s => s.ChangePasswordAsync(changePasswordDto, userId))
                    .ThrowsAsync(new InvalidOperationException("Ancien mot de passe incorrect."));

            // Act
            var result = await _accountController.ChangePasswordAsync(changePasswordDto);

            // Assert
            _mockAccountService.Verify(s => s.ChangePasswordAsync(changePasswordDto, userId), Times.Once);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ancien mot de passe incorrect.", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangePasswordAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mockAccountService.Setup(s => s.ChangePasswordAsync(changePasswordDto, userId))
                    .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _accountController.ChangePasswordAsync(changePasswordDto);

            // Assert
            _mockAccountService.Verify(s => s.ChangePasswordAsync(changePasswordDto, userId), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Change Email --
        [Fact]
        public async Task ChangeEmailAsync_DonneesValides_RetourneOk()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changeEmailDto = new ChangeEmailDTO
            {
                NewEmail = "newemail@example.com",
                ConfirmNewEmail = "newemail@example.com"
            };

            _mockAccountService.Setup(s => s.ChangeEmailAsync(changeEmailDto, userId))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.ChangeEmailAsync(changeEmailDto);

            // Assert
            _mockAccountService.Verify(s => s.ChangeEmailAsync(changeEmailDto, userId), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Adresse e-mail modifiée avec succès.", okResult.Value);
        }

        [Fact]
        public async Task ChangeEmailAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _accountController.ModelState.AddModelError("NewEmail", "L'adresse e-mail est requise.");

            // Act
            var result = await _accountController.ChangeEmailAsync(new ChangeEmailDTO());

            // Assert
            _mockAccountService.Verify(s => s.ChangeEmailAsync(It.IsAny<ChangeEmailDTO>(), It.IsAny<int>()), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangeEmailAsync_UtilisateurNonAuthentifie_RetourneBadRequest()
        {
            // Arrange
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // Utilisateur non authentifié
                }
            };

            // Act
            var result = await _accountController.ChangeEmailAsync(new ChangeEmailDTO());

            // Assert
            _mockAccountService.Verify(s => s.ChangeEmailAsync(It.IsAny<ChangeEmailDTO>(), It.IsAny<int>()), Times.Never);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Utilisateur non authentifié.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task ChangeEmailAsync_ServiceLanceInvalidOperationException_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changeEmailDto = new ChangeEmailDTO
            {
                NewEmail = "newemail@example.com",
                ConfirmNewEmail = "newemail@example.com"
            };

            _mockAccountService.Setup(s => s.ChangeEmailAsync(changeEmailDto, userId))
                    .ThrowsAsync(new InvalidOperationException("L'adresse e-mail est déjà utilisée."));

            // Act
            var result = await _accountController.ChangeEmailAsync(changeEmailDto);

            // Assert
            _mockAccountService.Verify(s => s.ChangeEmailAsync(changeEmailDto, userId), Times.Once);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("L'adresse e-mail est déjà utilisée.", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangeEmailAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            var changeEmailDto = new ChangeEmailDTO
            {
                NewEmail = "newemail@example.com",
                ConfirmNewEmail = "newemail@example.com"
            };

            _mockAccountService.Setup(s => s.ChangeEmailAsync(changeEmailDto, userId))
                    .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _accountController.ChangeEmailAsync(changeEmailDto);

            // Assert
            _mockAccountService.Verify(s => s.ChangeEmailAsync(changeEmailDto, userId), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // -- Delete Account --
        [Fact]
        public async Task DeleteAccountAsync_DonneesValides_RetourneOk()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _mockAccountService.Setup(s => s.DeleteAccountAsync(userId))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.DeleteAccountAsync();

            // Assert
            _mockAccountService.Verify(s => s.DeleteAccountAsync(userId), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Compte supprimé avec succès.", okResult.Value);
        }

        [Fact]
        public async Task DeleteAccountAsync_DonneesInvalides_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _accountController.ModelState.AddModelError("Email", "L'adresse e-mail est invalide.");

            // Act
            var result = await _accountController.DeleteAccountAsync();

            // Assert
            _mockAccountService.Verify(s => s.DeleteAccountAsync(userId), Times.Never);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Les données fournies sont invalides.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteAccountAsync_UtilisateurNonAuthentifie_RetourneBadRequest()
        {
            // Arrange
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // Utilisateur non authentifié
                }
            };

            // Act
            var result = await _accountController.DeleteAccountAsync();

            // Assert
            _mockAccountService.Verify(s => s.DeleteAccountAsync(It.IsAny<int>()), Times.Never);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Utilisateur non authentifié.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task DeleteAccountAsync_ServiceLanceInvalidOperationException_RetourneBadRequest()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _mockAccountService.Setup(s => s.DeleteAccountAsync(userId))
                    .ThrowsAsync(new InvalidOperationException("Utilisateur introuvable."));

            // Act
            var result = await _accountController.DeleteAccountAsync();

            // Assert
            _mockAccountService.Verify(s => s.DeleteAccountAsync(userId), Times.Once);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Utilisateur introuvable.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteAccountAsync_ServiceLanceException_RetourneInternalServerError()
        {
            // Arrange
            var userId = 1;

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreerUtilisateurTest(userId)
                }
            };

            _mockAccountService.Setup(s => s.DeleteAccountAsync(userId))
                    .ThrowsAsync(new Exception("Erreur inattendue."));

            // Act
            var result = await _accountController.DeleteAccountAsync();

            // Assert
            _mockAccountService.Verify(s => s.DeleteAccountAsync(userId), Times.Once);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Une erreur interne est survenue.", internalServerErrorResult.Value);
        }


        // Méthodes utilitaires
        private static ClaimsPrincipal CreerUtilisateurTest(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");

            return new ClaimsPrincipal(identity);
        }
    }
}
