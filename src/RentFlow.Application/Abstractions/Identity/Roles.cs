namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// The canonical role names used for role-based authorization. These strings are
/// the single source of truth shared by Identity seeding, JWT role claims and
/// <c>[Authorize(Roles = ...)]</c> attributes.
/// </summary>
public static class Roles
{
    /// <summary>The platform administrator with unrestricted access.</summary>
    public const string Admin = "Admin";

    /// <summary>A property owner who posts listings and reviews applications.</summary>
    public const string Owner = "Owner";

    /// <summary>A tenant who browses listings and submits rental applications.</summary>
    public const string Tenant = "Tenant";

    /// <summary>All role names, in seeding order.</summary>
    public static readonly IReadOnlyList<string> All = [Admin, Owner, Tenant];
}
