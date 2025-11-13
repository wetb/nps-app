
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Domain.Entities;

namespace NPSApplication.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString;

    public RefreshTokenRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using var connection = CreateConnection();
        
        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "sp_GetRefreshToken",
            new { Token = token },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<int> CreateAsync(RefreshToken refreshToken)
    {
        using var connection = CreateConnection();
        
        return await connection.ExecuteScalarAsync<int>(
            "sp_SaveRefreshToken",
            new { 
                UserId = refreshToken.UserId, 
                Token = refreshToken.Token, 
                ExpiresAt = refreshToken.ExpiresAt 
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task RevokeAsync(string token, string? replacedByToken = null)
    {
        using var connection = CreateConnection();
        
        await connection.ExecuteAsync(
            "sp_RevokeRefreshToken",
            new { Token = token, ReplacedByToken = replacedByToken },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            UPDATE RefreshTokens 
            SET IsRevoked = 1, RevokedAt = GETUTCDATE(), UpdatedAt = GETUTCDATE()
            WHERE UserId = @UserId AND IsRevoked = 0";

        await connection.ExecuteAsync(sql, new { UserId = userId });
    }
}