namespace RentFlow.API.Applications;

/// <summary>The request body for submitting a rental application.</summary>
/// <param name="PropertyId">The identifier of the property being applied for.</param>
/// <param name="Message">An optional message to the owner.</param>
public sealed record SubmitApplicationRequest(Guid PropertyId, string? Message);
