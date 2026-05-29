namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// The role a user may self-register as. The privileged <c>Admin</c> role is never
/// self-assignable; it is provisioned by seeding only.
/// </summary>
public enum AccountRole
{
    /// <summary>A property owner who posts listings and reviews applications.</summary>
    Owner = 1,

    /// <summary>A tenant who browses listings and submits rental applications.</summary>
    Tenant = 2,
}
