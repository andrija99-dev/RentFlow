using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.Submit;

public sealed record SubmitApplicationCommand(
    Guid PropertyId,
    string? Message) : ICommand<RentalApplicationResponse>;
