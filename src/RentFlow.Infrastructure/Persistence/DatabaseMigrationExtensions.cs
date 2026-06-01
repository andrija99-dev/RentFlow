using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RentFlow.Infrastructure.Persistence;

/// <summary>
/// Host extensions that apply pending EF Core migrations on application startup.
/// Ensures the target database exists and is at the latest schema version before
/// the API begins accepting requests.
/// </summary>
public static class DatabaseMigrationExtensions
{
    /// <summary>
    /// Creates the database if it does not exist and applies all pending migrations.
    /// Logs the set of migrations that were applied (or notes that none were pending).
    /// </summary>
    public static async Task ApplyDatabaseMigrationsAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RentFlowDbContext>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<RentFlowDbContext>();

        var pendingMigrations = (await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken)
            .ConfigureAwait(false))
            .ToList();

        if (pendingMigrations.Count == 0)
        {
            logger.LogInformation("Database schema is up to date; no pending migrations.");
            return;
        }

        logger.LogInformation(
            "Applying {Count} pending migration(s): {Migrations}",
            pendingMigrations.Count,
            string.Join(", ", pendingMigrations));

        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Database migrations applied successfully.");
    }
}
