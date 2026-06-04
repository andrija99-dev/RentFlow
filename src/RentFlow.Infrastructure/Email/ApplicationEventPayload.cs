namespace RentFlow.Infrastructure.Email;

/// <summary>
/// The shared shape of the rental-application status events (submitted, accepted,
/// rejected) as carried in the outbox payload, used to address the notification.
/// </summary>
/// <param name="ApplicationId">The application identifier.</param>
/// <param name="PropertyId">The property the application targets.</param>
/// <param name="TenantId">The tenant who submitted the application.</param>
internal sealed record ApplicationEventPayload(Guid ApplicationId, Guid PropertyId, Guid TenantId);
