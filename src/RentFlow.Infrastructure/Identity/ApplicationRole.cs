using Microsoft.AspNetCore.Identity;

namespace RentFlow.Infrastructure.Identity;

/// <summary>
/// The application's role entity, extending the ASP.NET Identity role with a
/// <see cref="Guid"/> primary key to match <see cref="ApplicationUser"/>.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>Initializes a new role with no name (required by the store).</summary>
    public ApplicationRole()
    {
    }

    /// <summary>Initializes a new role with the given name.</summary>
    /// <param name="roleName">The role name.</param>
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
