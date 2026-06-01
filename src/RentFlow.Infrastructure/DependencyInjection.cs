using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Documents;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Abstractions.Storage;
using RentFlow.Domain.Interfaces;
using RentFlow.Infrastructure.Caching;
using RentFlow.Infrastructure.Documents;
using RentFlow.Infrastructure.Identity;
using RentFlow.Infrastructure.Persistence;
using RentFlow.Infrastructure.Persistence.Reads;
using RentFlow.Infrastructure.Persistence.Repositories;
using RentFlow.Infrastructure.Storage;
using StackExchange.Redis;

namespace RentFlow.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Registers persistence,
/// caching, messaging, storage, email and background-job services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>The configuration key for the primary PostgreSQL connection string.</summary>
    public const string DefaultConnectionStringName = "RentFlowDb";

    /// <summary>The configuration key for the optional Redis connection string.</summary>
    public const string RedisConnectionStringName = "Redis";

    /// <summary>The configuration key for the optional Azure Blob Storage connection string.</summary>
    public const string BlobStorageConnectionStringName = "BlobStorage";

    /// <summary>Adds the Infrastructure layer services to the dependency injection container.</summary>
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

        services.AddSingleton<ISqlConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
        services.AddScoped<IPropertyReadService, PropertyReadService>();
        services.AddScoped<IRentalApplicationReadService, RentalApplicationReadService>();
        services.AddScoped<IContractReadService, ContractReadService>();

        services.AddCaching(configuration);
        services.AddStorage(configuration);
        services.AddAuthenticationServices(configuration);

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));
        services.AddSingleton<IContractDocumentGenerator, ContractDocumentGenerator>();

        var blobConnectionString = configuration.GetConnectionString(BlobStorageConnectionStringName);

        if (string.IsNullOrWhiteSpace(blobConnectionString))
        {
            services.AddSingleton<IDocumentStorage, LocalFileDocumentStorage>();
            return services;
        }

        services.AddSingleton(_ => new BlobServiceClient(blobConnectionString));
        services.AddSingleton<IDocumentStorage, AzureBlobDocumentStorage>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString(RedisConnectionStringName);

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddSingleton<ICacheService, NullCacheService>();
            return services;
        }

        var options = ConfigurationOptions.Parse(redisConnectionString);
        options.AbortOnConnectFail = false;

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options));
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);

        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                $"The '{JwtOptions.SectionName}' configuration section is missing.");

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey)
            || Encoding.UTF8.GetByteCount(jwtOptions.SigningKey) < 32)
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)}' must be configured "
                + "and at least 32 bytes long for HMAC-SHA256 signing.");
        }

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<RentFlowDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<JwtTokenGenerator>();

        return services;
    }
}
