using System.Net;
using System.Security.Claims;
using System.Text.Json;
using TP3_API.Data;
using TP3_API.Models;

namespace TP3_API.Middlewares
{
    public class AuditoriaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditoriaMiddleware> _logger;

        public AuditoriaMiddleware(RequestDelegate next, ILogger<AuditoriaMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            try
            {
                // Continuar con la petición normal de la API
                await _next(context);

                // Opcional: Registrar peticiones exitosas importantes (ej: POST, PUT, DELETE)
                if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")
                {
                    if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    {
                        _logger.LogInformation("Acción exitosa en {Path} por método {Method}", context.Request.Path, context.Request.Method);
                    }
                }
            }
            catch (Exception ex)
            {
                // 1. Registrar con Serilog (escribe en el archivo de texto en /Logs)
                _logger.LogError(ex, "Error no controlado en {Path}: {Message}", context.Request.Path, ex.Message);

                // 2. Guardar en la base de datos usando tu modelo LogAuditoria
                await RegistrarLogEnDbAsync(context, dbContext, ex);

                // 3. Devolver respuesta JSON limpia al cliente
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task RegistrarLogEnDbAsync(HttpContext context, AppDbContext dbContext, Exception ex)
        {
            try
            {
                int? usuarioId = null;
                var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User?.FindFirst("id")?.Value;
                if (int.TryParse(userIdClaim, out var parsedId))
                {
                    usuarioId = parsedId;
                }

                var log = new LogAuditoria
                {
                    Fecha = DateTime.Now,
                    Nivel = "Error",
                    Mensaje = $"Ruta: {context.Request.Path} | Excepción: {ex.Message}",
                    StackTrace = ex.StackTrace,
                    UsuarioId = usuarioId
                };

                dbContext.LogsAuditoria.Add(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "No se pudo persistir el log de auditoría en la base de datos.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                status = context.Response.StatusCode,
                mensaje = "Ocurrió un error interno en el servidor.",
                detalles = exception.Message
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}