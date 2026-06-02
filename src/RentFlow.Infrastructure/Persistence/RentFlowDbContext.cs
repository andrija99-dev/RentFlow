using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DomainEvents;
using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Infrastructure.Identity;
using RentFlow.Infrastructure.Outbox;

namespace RentFlow.Infrastructure.Persistence;

/// <inheritdoc />
public sealed class RentFlowDbContext(
    DbContextOptions<RentFlowDbContext> options,
    IPublisher publisher) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    private readonly IPublisher _publisher = publisher;

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<RentalApplication> RentalApplications => Set<RentalApplication>();

    public DbSet<Contract> Contracts => Set<Contract>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentFlowDbContext).Assembly);
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

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(OutboxMessageFactory.Create(domainEvent));
        }

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
