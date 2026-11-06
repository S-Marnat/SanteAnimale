using System.Diagnostics;
using System.Security.Claims;

namespace SanteAnimale.Auth.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Démarrer le chronomètre
            var stopwatch = Stopwatch.StartNew();

            // Récupérer les infos de la requête
            var httpMethod = context.Request.Method;
            var path = context.Request.Path;

            // Récupérer l'Id utilisateur pseudonymisé (si authentification)
            string? userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Log d’entrée
            _logger.LogInformation(
                "Requete entrante pour la méthode {Method}, le chemin {Path}, et l'utilisateur {UserId}",
                httpMethod,
                path,
                userId ?? "anonyme"
            );

            // Laisser l’API traiter la requête
            await _next(context);

            // Arrêter le chronomètre
            stopwatch.Stop();

            // Récupérer le statut HTTP
            var statusCode = context.Response.StatusCode;

            // Log de sortie
            _logger.LogInformation(
                "Reponse sortante pour la méthode {Method}, le chemin {Path}, le code de statut {StatusCode}, et la durée de {Duration}",
                httpMethod,
                path,
                statusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
