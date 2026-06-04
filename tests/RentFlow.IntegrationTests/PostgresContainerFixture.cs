using MediatR;
using Microsoft.EntityFrameworkCore;
using RentFlow.Infrastructure.Persistence;
using RentFlow.Infrastructure.Persistence.Reads;
using Testcontainers.PostgreSql;

namespace RentFlow.IntegrationTests;

/// <summary>
/// Spins up a throwaway PostgreSQL container once per test collection, applies the
/// EF Core migrations, and hands out fresh <see cref="RentFlowDbContext"/> instances
/// and a Dapper connection factory pointed at it. Requires a running Docker engine.
/// </summary>
public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    private string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    /// <summary>Creates a fresh DbContext against the container (DbContext is not thread-safe).</summary>
    public RentFlowDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RentFlowDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new RentFlowDbContext(options, Mock.Of<IPublisher>());
    }

    /// <summary>Creates a Dapper connection factory against the container.</summary>
    internal ISqlConnectionFactory CreateConnectionFactory() => new NpgsqlConnectionFactory(ConnectionString);
}

/// <summary>Shares a single PostgreSQL container across all integration tests.</summary>
[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
    public const string Name = "postgres";
}
