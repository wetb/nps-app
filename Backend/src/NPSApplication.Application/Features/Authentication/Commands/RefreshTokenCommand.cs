using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Application.DTOs;
using MediatR;

namespace NPSApplication.Application.Features.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) 
    : IRequest<TokenResponse>;

public class RefreshTokenCommandHandler 
    : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> Handle(
        RefreshTokenCommand request, 
        CancellationToken cancellationToken)
    {
        return await _tokenService.RefreshTokenAsync(request.RefreshToken);
    }
}