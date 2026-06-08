using MediatR;
using RentFlow.Domain.Common;

namespace RentFlow.Application.DomainEvents;

/// <summary>
/// Adapts a domain <see cref="IDomainEvent"/> to a MediatR <see cref="INotification"/>
/// so it can be published to one or more handlers. The infrastructure publishes
/// these after the unit of work commits successfully.
/// </summary>
/// <typeparam name="TDomainEvent">The wrapped domain event type.</typeparam>
public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : IDomainEvent
{
    /// <summary>Gets the wrapped domain event.</summary>
    public TDomainEvent DomainEvent { get; } = domainEvent;
}
