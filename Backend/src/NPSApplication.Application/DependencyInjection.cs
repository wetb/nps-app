using NPSApplication.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using AutoMapper;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NPSApplication.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<DTOs.AuthenticationResponse>();

        services.AddAutoMapper(typeof(DependencyInjection));

        return services;
    }
}