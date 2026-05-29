using RentFlow.Application.Abstractions.Identity;

namespace RentFlow.API.Authentication;

/// <summary>The request body for registering a new account.</summary>
/// <param name="Email">The email address, used as the unique user name.</param>
/// <param name="Password">The plain-text password.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Role">The role to register as (<c>Owner</c> or <c>Tenant</c>).</param>
public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    AccountRole Role);

/// <summary>The request body for authenticating with email and password.</summary>
/// <param name="Email">The email address identifying the account.</param>
/// <param name="Password">The plain-text password.</param>
public sealed record LoginRequest(string Email, string Password);

/// <summary>The request body carrying a refresh token to exchange or revoke.</summary>
/// <param name="RefreshToken">The refresh token issued to the client.</param>
public sealed record RefreshTokenRequest(string RefreshToken);
