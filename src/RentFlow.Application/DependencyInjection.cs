using Microsoft.Extensions.DependencyInjection;

namespace RentFlow.Application;

/// <summary>
/// Composition root for the Application layer. Registers CQRS handlers,
/// validators, mapping profiles and MediatR pipeline behaviors.
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
        // MediatR, FluentValidation and AutoMapper registrations are added
        // in the feature/application-layer branch.
        return services;
    }
}
