
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Domain.Entities;

namespace NPSApplication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            SELECT Id, Username, PasswordHash, Role, IsLocked, 
                   FailedLoginAttempts, LastLoginAttempt, LastSuccessfulLogin,
                   IsActive, CreatedAt, UpdatedAt
            FROM Users
            WHERE Id = @Id AND IsActive = 1";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            SELECT Id, Username, PasswordHash, Role, IsLocked, 
                   FailedLoginAttempts, LastLoginAttempt, LastSuccessfulLogin,
                   IsActive, CreatedAt, UpdatedAt
            FROM Users
            WHERE Username = @Username AND IsActive = 1";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<User?> AuthenticateAsync(string username)
    {
        using var connection = CreateConnection();
        
        return await connection.QueryFirstOrDefaultAsync<User>(
            "sp_AuthenticateUser",
            new { Username = username },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task RegisterFailedLoginAsync(int userId)
    {
        using var connection = CreateConnection();
        
        await connection.ExecuteAsync(
            "sp_RegisterFailedLogin",
            new { UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task RegisterSuccessfulLoginAsync(int userId)
    {
        using var connection = CreateConnection();
        
        await connection.ExecuteAsync(
            "sp_RegisterSuccessfulLogin",
            new { UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
            VALUES (@Username, @PasswordHash, @Role, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            UPDATE Users 
            SET Username = @Username,
                Role = @Role,
                IsLocked = @IsLocked,
                FailedLoginAttempts = @FailedLoginAttempts,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, user);
    }
}