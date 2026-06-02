using RabbitMQ.Client;

namespace RentFlow.Infrastructure.Messaging;

/// <summary>
/// A shared, lazily-established RabbitMQ connection. Connecting is deferred until the
/// first use so a broker that is down at start-up never crashes the host; callers
/// create their own channels from the returned connection.
/// </summary>
internal interface IRabbitMqConnection : IAsyncDisposable
{
    /// <summary>Returns the open connection, establishing it on first use.</summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}
