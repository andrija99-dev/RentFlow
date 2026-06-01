using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.Submit;

/// <summary>Submits a rental application for a property on behalf of the current tenant.</summary>
/// <param name="PropertyId">The identifier of the property being applied for.</param>
/// <param name="Message">An optional message to the owner.</param>
public sealed record SubmitApplicationCommand(
    Guid PropertyId,
    string? Message) : ICommand<RentalApplicationResponse>;
