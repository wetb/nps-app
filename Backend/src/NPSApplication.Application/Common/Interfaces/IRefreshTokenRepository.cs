using NPSApplication.Domain.Entities;
namespace NPSApplication.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<int> CreateAsync(RefreshToken refreshToken);
    Task RevokeAsync(string token, string? replacedByToken = null);
    Task RevokeAllUserTokensAsync(int userId);
}