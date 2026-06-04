using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.RentalApplications.Common;

/// <summary>
/// Authorization guards for rental applications. Deciding an application (accept or
/// reject) is the property owner's right and reuses the property guard; the guards
/// here cover the tenant-facing actions of withdrawing and viewing.
/// </summary>
internal static class ApplicationAuthorization
{
    /// <summary>Throws unless the current user is the tenant who submitted the application (or an admin).</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the applicant nor an administrator.</exception>
    public static void EnsureIsApplicant(Guid tenantId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin) || currentUser.UserId == tenantId)
        {
            return;
        }

        throw new ForbiddenAccessException("You can only act on your own applications.");
    }

    /// <summary>Throws unless the current user is the applicant, the property owner, or an admin.</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is none of the permitted parties.</exception>
    public static void EnsureCanView(Guid tenantId, Guid propertyOwnerId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin)
            || currentUser.UserId == tenantId
            || currentUser.UserId == propertyOwnerId)
        {
            return;
        }

        throw new ForbiddenAccessException("You do not have access to this application.");
    }
}
