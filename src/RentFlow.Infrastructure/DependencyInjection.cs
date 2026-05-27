using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RentFlow.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Registers persistence,
/// caching, messaging, storage, email and background-job services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration used to bind options and connection strings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core, Dapper, Redis, RabbitMQ, Azure Blob, Hangfire and the outbox
        // are registered here in their respective feature branches.
        return services;
    }
}
