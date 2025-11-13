using NPSApplication.Domain.Entities;
using NPSApplication.Application.DTOs;
namespace NPSApplication.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string token);
}