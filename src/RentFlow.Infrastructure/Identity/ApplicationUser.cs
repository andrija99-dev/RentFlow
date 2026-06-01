using Microsoft.AspNetCore.Identity;

namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// The application's user entity, extending the ASP.NET Identity user with a
/// <see cref="Guid"/> primary key (to align with the platform's Guid v7 ids) and
/// profile fields.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; } = [];
}
