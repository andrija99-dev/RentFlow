namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// A persisted refresh token. Tokens are rotated on use — the presented token is
/// revoked and linked to its successor — so a stolen token is detectable and a
/// session can be terminated by revoking the active token.
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public Guid UserId { get; init; }

    public string Token { get; init; } = null!;

    public DateTime ExpiresAtUtc { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public DateTime? RevokedAtUtc { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsActive => RevokedAtUtc is null && !IsExpired;
}
