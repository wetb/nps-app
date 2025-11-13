using NPSApplication.Domain.Entities;
namespace NPSApplication.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> AuthenticateAsync(string username);
    Task RegisterFailedLoginAsync(int userId);
    Task RegisterSuccessfulLoginAsync(int userId);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
}