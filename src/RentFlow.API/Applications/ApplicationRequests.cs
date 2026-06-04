namespace RentFlow.API.Applications;

public sealed record SubmitApplicationRequest(Guid PropertyId, string? Message);
