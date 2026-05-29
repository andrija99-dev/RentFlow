namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// A persisted refresh token. Tokens are rotated on use — the presented token is
/// revoked and linked to its successor — so a stolen token is detectable and a
/// session can be terminated by revoking the active token.
/// </summary>
public sealed class RefreshToken
{
    /// <summary>Gets the surrogate primary key.</summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>Gets the identifier of the user the token was issued to.</summary>
    public Guid UserId { get; init; }

    /// <summary>Gets the opaque, cryptographically random token value.</summary>
    public string Token { get; init; } = null!;

    /// <summary>Gets the UTC instant at which the token expires.</summary>
    public DateTime ExpiresAtUtc { get; init; }

    /// <summary>Gets the UTC instant at which the token was created.</summary>
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    /// <summary>Gets or sets the UTC instant at which the token was revoked, if any.</summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>Gets or sets the token that replaced this one on rotation, if any.</summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>Gets a value indicating whether the token has expired.</summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    /// <summary>Gets a value indicating whether the token is currently usable.</summary>
    public bool IsActive => RevokedAtUtc is null && !IsExpired;
}
