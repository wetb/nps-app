using NPSApplication.Application.Common.Interfaces;
using MediatR;

namespace NPSApplication.Application.Features.Authentication.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<bool>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> Handle(
        LogoutCommand request, 
        CancellationToken cancellationToken)
    {
        await _tokenService.RevokeTokenAsync(request.RefreshToken);
        return true;
    }
}
