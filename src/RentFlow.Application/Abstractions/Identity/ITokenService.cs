namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// Issues and rotates the access/refresh token pair. The access token is a signed
/// JWT; the refresh token is persisted so it can be rotated and revoked.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Mints a fresh access token and a new persisted refresh token for the user.
    /// </summary>
    /// <param name="user">The authenticated user the tokens are issued for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The issued token pair.</returns>
    Task<AuthenticationResult> IssueTokensAsync(
        AuthenticatedUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and rotates a refresh token: the presented token is revoked and a
    /// new access/refresh pair is issued (refresh-token rotation).
    /// </summary>
    /// <param name="refreshToken">The refresh token presented by the client.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly issued token pair.</returns>
    /// <exception cref="Exceptions.AuthenticationException">Thrown when the refresh token is unknown, expired or already used.</exception>
    Task<AuthenticationResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token so it can no longer be used (logout). Revoking an
    /// unknown or already-revoked token is a no-op.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}
