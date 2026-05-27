using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;

namespace RentFlow.Domain.Entities;

/// <summary>
/// A tenant's application to rent a property. Aggregate root that enforces the
/// pending → accepted/rejected/withdrawn transition and raises the corresponding
/// domain events.
/// </summary>
public sealed class RentalApplication : AggregateRoot
{
    private RentalApplication(Guid id, Guid propertyId, Guid tenantId, string? message) : base(id)
    {
        PropertyId = propertyId;
        TenantId = tenantId;
        Message = message;
        Status = ApplicationStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private RentalApplication()
    {
    }

    /// <summary>Gets the identifier of the property applied for.</summary>
    public Guid PropertyId { get; private set; }

    /// <summary>Gets the identifier of the applying tenant.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Gets the current status of the application.</summary>
    public ApplicationStatus Status { get; private set; }

    /// <summary>Gets the optional message the tenant included with the application.</summary>
    public string? Message { get; private set; }

    /// <summary>Gets the UTC timestamp at which the application was submitted.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Gets the UTC timestamp at which the application was decided, if any.</summary>
    public DateTime? DecidedAtUtc { get; private set; }

    /// <summary>Submits a new rental application in the <see cref="ApplicationStatus.Pending"/> state.</summary>
    /// <param name="propertyId">The identifier of the property being applied for.</param>
    /// <param name="tenantId">The identifier of the applying tenant.</param>
    /// <param name="message">An optional message to the owner.</param>
    /// <returns>The created <see cref="RentalApplication"/>.</returns>
    /// <exception cref="DomainException">Thrown when the property or tenant identifier is empty.</exception>
    public static RentalApplication Create(Guid propertyId, Guid tenantId, string? message)
    {
        if (propertyId == Guid.Empty)
        {
            throw new DomainException("A rental application must reference a property.");
        }

        if (tenantId == Guid.Empty)
        {
            throw new DomainException("A rental application must reference a tenant.");
        }

        var application = new RentalApplication(Guid.CreateVersion7(), propertyId, tenantId, message?.Trim());
        application.RaiseDomainEvent(new RentalApplicationSubmittedEvent(application.Id, propertyId, tenantId));
        return application;
    }

    /// <summary>Accepts the application, transitioning it to <see cref="ApplicationStatus.Accepted"/>.</summary>
    /// <exception cref="DomainException">Thrown when the application is not pending.</exception>
    public void Accept()
    {
        EnsurePending();
        Status = ApplicationStatus.Accepted;
        DecidedAtUtc = DateTime.UtcNow;
        RaiseDomainEvent(new RentalApplicationAcceptedEvent(Id, PropertyId, TenantId));
    }

    /// <summary>Rejects the application, transitioning it to <see cref="ApplicationStatus.Rejected"/>.</summary>
    /// <exception cref="DomainException">Thrown when the application is not pending.</exception>
    public void Reject()
    {
        EnsurePending();
        Status = ApplicationStatus.Rejected;
        DecidedAtUtc = DateTime.UtcNow;
        RaiseDomainEvent(new RentalApplicationRejectedEvent(Id, PropertyId, TenantId));
    }

    /// <summary>Withdraws the application on the tenant's behalf.</summary>
    /// <exception cref="DomainException">Thrown when the application is not pending.</exception>
    public void Withdraw()
    {
        EnsurePending();
        Status = ApplicationStatus.Withdrawn;
        DecidedAtUtc = DateTime.UtcNow;
    }

    private void EnsurePending()
    {
        if (Status != ApplicationStatus.Pending)
        {
            throw new DomainException($"Only pending applications can change state; current status is {Status}.");
        }
    }
}
