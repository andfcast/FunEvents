using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FunEvents.Domain.Exceptions;

namespace FunEvents.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                DomainException => (StatusCodes.Status400BadRequest, "Error en Regla de Negocio"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "Operación cancelada por el cliente"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Indica que la excepción fue manejada
        }
    }
}
