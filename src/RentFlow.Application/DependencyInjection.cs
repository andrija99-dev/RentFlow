using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Application.Behaviors;

namespace RentFlow.Application;

/// <summary>
/// Composition root for the Application layer. Registers MediatR handlers, the
/// pipeline behaviors, FluentValidation validators and AutoMapper profiles found
/// in this assembly.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);

            configuration.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddAutoMapper(configuration => configuration.AddMaps(assembly));

        return services;
    }
}
