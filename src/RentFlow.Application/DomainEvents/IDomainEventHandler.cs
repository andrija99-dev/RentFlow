using MediatR;
using RentFlow.Domain.Common;

namespace RentFlow.Application.DomainEvents;

/// <summary>
/// Handles a domain event. An ergonomic alias over
/// <see cref="INotificationHandler{TNotification}"/> for the wrapped event, so
/// handlers declare the domain event type directly.
/// </summary>
/// <typeparam name="TDomainEvent">The domain event type handled.</typeparam>
public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent;
