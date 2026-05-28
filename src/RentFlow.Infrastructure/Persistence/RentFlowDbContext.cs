using MediatR;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DomainEvents;
using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence;

/// <inheritdoc />
public sealed class RentFlowDbContext(
    DbContextOptions<RentFlowDbContext> options,
    IPublisher publisher) : DbContext(options)
{
    private readonly IPublisher _publisher = publisher;

    /// <summary>Gets the set of property listings.</summary>
    public DbSet<Property> Properties => Set<Property>();

    /// <summary>Gets the set of rental applications.</summary>
    public DbSet<RentalApplication> RentalApplications => Set<RentalApplication>();

    /// <summary>Gets the set of rental contracts.</summary>
    public DbSet<Contract> Contracts => Set<Contract>();

    /// <summary>Gets the set of scheduled payments.</summary>
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>Gets the set of webhook subscriptions.</summary>
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = aggregatesWithEvents
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var aggregate in aggregatesWithEvents)
        {
            aggregate.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
            await _publisher.Publish(notification, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }
}
