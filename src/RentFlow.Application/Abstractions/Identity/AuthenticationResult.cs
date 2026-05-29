namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// The pair of tokens issued on a successful authentication, returned to the client.
/// The access token authorizes API calls; the refresh token obtains a new pair once
/// the access token expires.
/// </summary>
/// <param name="AccessToken">The signed JWT bearer token.</param>
/// <param name="AccessTokenExpiresAtUtc">The UTC instant at which the access token expires.</param>
/// <param name="RefreshToken">The opaque refresh token used to obtain a new token pair.</param>
/// <param name="RefreshTokenExpiresAtUtc">The UTC instant at which the refresh token expires.</param>
public sealed record AuthenticationResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc)
{
    /// <summary>The token scheme clients use in the <c>Authorization</c> header.</summary>
    public string TokenType => "Bearer";
}
