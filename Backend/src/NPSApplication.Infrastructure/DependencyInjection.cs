
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPSApplication.Application.Common.Interfaces;
using NPSApplication.Infrastructure.Repositories;
using NPSApplication.Infrastructure.Services;
using NPSApplication.Infrastructure.Data;

namespace NPSApplication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}