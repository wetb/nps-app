using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Domain.Entities;
using NPSApplication.Application.DTOs;
using MediatR;

namespace NPSApplication.Application.Features.Authentication.Commands;

public record LoginCommand(string Username, string Password) 
    : IRequest<AuthenticationResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationResponse> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken)
    {
        // Obtener usuario
        var user = await _unitOfWork.Users.AuthenticateAsync(request.Username);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        // Verificar si está bloqueado
        if (user.IsLocked)
            throw new UnauthorizedAccessException("Cuenta bloqueada. Contacte al administrador");

        // Verificar contraseña
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            await _unitOfWork.Users.RegisterFailedLoginAsync(user.Id);
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        // Login exitoso
        await _unitOfWork.Users.RegisterSuccessfulLoginAsync(user.Id);

        // Generar tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Guardar refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.CreateAsync(refreshTokenEntity);

        return new AuthenticationResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 300 // 5 minutos
        };
    }
}