namespace RentFlow.Domain.Common;

/// <summary>
/// Base class for aggregate roots — the only entities that may be loaded and
/// persisted through a repository. Records the domain events raised during a
/// unit of work so they can be dispatched once changes are committed.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Initializes a new aggregate root with the supplied identifier.</summary>
    /// <param name="id">The unique identifier of the aggregate.</param>
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>Parameterless constructor reserved for the EF Core materializer.</summary>
    protected AggregateRoot()
    {
    }

    /// <summary>Gets the domain events raised by this aggregate that have not yet been dispatched.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Records a domain event to be dispatched after the current unit of work is committed.</summary>
    /// <param name="domainEvent">The event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Clears the recorded domain events. Called by the infrastructure after dispatching them.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
