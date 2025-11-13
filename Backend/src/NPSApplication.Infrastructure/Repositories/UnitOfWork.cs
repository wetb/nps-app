
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPSApplication.Application.Common.Interfaces;

namespace NPSApplication.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;
    
    public IUserRepository Users { get; }
    public IVoteRepository Votes { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(
        IConfiguration configuration,
        IUserRepository userRepository,
        IVoteRepository voteRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        Users = userRepository;
        Votes = voteRepository;
        RefreshTokens = refreshTokenRepository;
    }

    public async Task<int> CommitAsync()
    {
        // Con Dapper, cada operación es independiente
        // Este método es principalmente para mantener el patrón
        return await Task.FromResult(0);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}