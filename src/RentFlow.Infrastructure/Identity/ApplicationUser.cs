using Microsoft.AspNetCore.Identity;

namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// The application's user entity, extending the ASP.NET Identity user with a
/// <see cref="Guid"/> primary key (to align with the platform's Guid v7 ids) and
/// profile fields.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Gets or sets the user's first name.</summary>
    public string FirstName { get; set; } = null!;

    /// <summary>Gets or sets the user's last name.</summary>
    public string LastName { get; set; } = null!;

    /// <summary>Gets the refresh tokens issued to this user.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; } = [];
}
