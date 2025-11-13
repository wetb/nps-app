namespace NPSApplication.API.Middleware;

using Microsoft.AspNetCore.Authorization;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir peticiones OPTIONS sin validación
        if (context.Request.Method == "OPTIONS")
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint();

        // Si no requiere autorización, continuar
        if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await _next(context);
            return;
        }

        // Verificar que tenga token
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Token de autenticación requerido"
            });
            return;
        }

        await _next(context);
    }
}