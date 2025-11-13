
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Domain.Entities;
using NPSApplication.Application.DTOs;

namespace NPSApplication.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly string _connectionString;

    public VoteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<Vote?> GetByUserIdAsync(int userId)
    {
        using var connection = CreateConnection();
        
        const string sql = @"
            SELECT Id, UserId, Score, Category, VotedAt, 
                   IsActive, CreatedAt, UpdatedAt
            FROM Votes
            WHERE UserId = @UserId AND IsActive = 1";

        return await connection.QueryFirstOrDefaultAsync<Vote>(sql, new { UserId = userId });
    }

    public async Task<bool> HasUserVotedAsync(int userId)
    {
        using var connection = CreateConnection();
        
        var result = await connection.ExecuteScalarAsync<int>(
            "sp_HasUserVoted",
            new { UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure
        );

        return result > 0;
    }

    public async Task<int> CreateAsync(Vote vote)
    {
        using var connection = CreateConnection();
        
        return await connection.ExecuteScalarAsync<int>(
            "sp_CreateVote",
            new { UserId = vote.UserId, Score = vote.Score },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<NPSResultResponse> GetNPSResultsAsync()
    {
        using var connection = CreateConnection();
        var result = await connection.QueryFirstAsync<NPSResultResponse>(
            "sp_GetNPSResults",
            commandType: System.Data.CommandType.StoredProcedure
        );
        result.CalculatePercentages();
        return result;
    }
}