using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Logout;

/// <summary>Revokes a refresh token so it can no longer be used (logout).</summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public sealed record RevokeTokenCommand(string RefreshToken) : ICommand;
