using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Domain.Interfaces;
using RentFlow.Infrastructure.Persistence;
using RentFlow.Infrastructure.Persistence.Repositories;

namespace RentFlow.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Registers persistence,
/// caching, messaging, storage, email and background-job services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>The configuration key for the primary PostgreSQL connection string.</summary>
    public const string DefaultConnectionStringName = "RentFlowDb";

    /// <summary>
    /// Adds the Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration used to bind options and connection strings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the required connection string is missing.</exception>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DefaultConnectionStringName}' is not configured.");

        services.AddDbContext<RentFlowDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(RentFlowDbContext).Assembly.GetName().Name)));

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IRentalApplicationRepository, RentalApplicationRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IWebhookSubscriptionRepository, WebhookSubscriptionRepository>();

        return services;
    }
}
