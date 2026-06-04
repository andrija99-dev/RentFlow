namespace RentFlow.Domain.Common;

/// <summary>
/// Base class for aggregate roots — the only entities that may be loaded and
/// persisted through a repository. Records the domain events raised during a
/// unit of work so they can be dispatched once changes are committed.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>Parameterless constructor reserved for the EF Core materializer.</summary>
    protected AggregateRoot()
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Records a domain event to be dispatched after the current unit of work is committed.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Clears the recorded domain events. Called by the infrastructure after dispatching them.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
