namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// A lightweight projection of an authenticated user, carrying just the data the
/// token service needs to mint a JWT. Decouples the Application layer from the
/// concrete ASP.NET Identity user type, which lives in Infrastructure.
/// </summary>
/// <param name="Id">The user's unique identifier.</param>
/// <param name="Email">The user's email address, used as the user name.</param>
/// <param name="Roles">The roles granted to the user.</param>
public sealed record AuthenticatedUser(Guid Id, string Email, IReadOnlyList<string> Roles);
