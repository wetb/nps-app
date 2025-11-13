using NPSApplication.Domain.Entities;
using NPSApplication.Application.DTOs;
namespace NPSApplication.Application.Common.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetByUserIdAsync(int userId);
    Task<bool> HasUserVotedAsync(int userId);
    Task<int> CreateAsync(Vote vote);
    Task<NPSResultResponse> GetNPSResultsAsync();
}