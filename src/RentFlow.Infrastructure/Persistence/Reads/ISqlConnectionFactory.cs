using System.Data;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <summary>
/// Creates open ADO.NET connections for the Dapper-based read side. Kept separate
/// from the EF Core context so read queries can run against the database without
/// going through the change tracker.
/// </summary>
internal interface ISqlConnectionFactory
{
    /// <summary>Opens and returns a new database connection.</summary>
    /// <param name="cancellationToken">A token to cancel the connect operation.</param>
    /// <returns>An open <see cref="IDbConnection"/> the caller is responsible for disposing.</returns>
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
