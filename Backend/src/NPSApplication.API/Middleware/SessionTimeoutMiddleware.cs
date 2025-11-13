using System.IdentityModel.Tokens.Jwt;

using System.Text.Json;

namespace NPSApplication.API.Middleware;

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionTimeoutMiddleware> _logger;

    public SessionTimeoutMiddleware(
        RequestDelegate next,
        ILogger<SessionTimeoutMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var expirationTime = jwtToken.ValidTo;

                if (expirationTime < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            message = "Sesión expirada. Por favor, inicie sesión nuevamente."
                        }));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al validar token de sesión");
            }
        }

        await _next(context);
    }
}