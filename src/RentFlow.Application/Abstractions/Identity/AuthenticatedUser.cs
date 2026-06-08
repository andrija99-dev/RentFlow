namespace RentFlow.Application.Abstractions.Identity;

public sealed record AuthenticatedUser(Guid Id, string Email, IReadOnlyList<string> Roles);
