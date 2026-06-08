using System.ComponentModel.DataAnnotations;

namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// Strongly-typed JWT settings bound from the <c>Jwt</c> configuration section.
/// Shared by the token issuer (Infrastructure) and the bearer validation setup (API).
/// </summary>
public sealed class JwtOptions
{
    /// <summary>The configuration section these options bind from.</summary>
    public const string SectionName = "Jwt";

    /// <summary>The token issuer (the <c>iss</c> claim).</summary>
    [Required]
    public string Issuer { get; init; } = null!;

    /// <summary>The intended token audience (the <c>aud</c> claim).</summary>
    [Required]
    public string Audience { get; init; } = null!;

    /// <summary>The symmetric signing key. Must be long enough for HMAC-SHA256 (≥ 32 bytes).</summary>
    [Required]
    [MinLength(32)]
    public string SigningKey { get; init; } = null!;

    /// <summary>The access token lifetime, in minutes.</summary>
    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 15;

    /// <summary>The refresh token lifetime, in days.</summary>
    [Range(1, 365)]
    public int RefreshTokenDays { get; init; } = 7;
}
