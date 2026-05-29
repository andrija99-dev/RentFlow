using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.RefreshToken;

/// <summary>Exchanges a valid refresh token for a new access/refresh token pair.</summary>
/// <param name="RefreshToken">The refresh token previously issued to the client.</param>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthenticationResult>;
