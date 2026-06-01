using RentFlow.Application.Abstractions.Identity;

namespace RentFlow.API.Authentication;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    AccountRole Role);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);
