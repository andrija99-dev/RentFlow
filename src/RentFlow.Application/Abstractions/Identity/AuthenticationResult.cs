namespace RentFlow.Application.Abstractions.Identity;

public sealed record AuthenticationResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc)
{
    public string TokenType => "Bearer";
}
