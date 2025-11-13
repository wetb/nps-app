namespace NPSApplication.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IVoteRepository Votes { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> CommitAsync();
}