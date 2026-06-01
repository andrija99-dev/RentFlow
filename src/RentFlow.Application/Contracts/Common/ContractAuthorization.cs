using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.Contracts.Common;

/// <summary>
/// Authorization guards for contracts. A contract may be viewed by either party to
/// it — the tenant or the property owner — or an admin; managing it (for example,
/// terminating it) is the owner's right.
/// </summary>
internal static class ContractAuthorization
{
    /// <summary>Throws unless the current user is the tenant, the property owner, or an admin.</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is none of the permitted parties.</exception>
    public static void EnsureCanView(Guid tenantId, Guid propertyOwnerId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin)
            || currentUser.UserId == tenantId
            || currentUser.UserId == propertyOwnerId)
        {
            return;
        }

        throw new ForbiddenAccessException("You do not have access to this contract.");
    }

    /// <summary>Throws unless the current user owns the rented property (or is an admin).</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the owner nor an administrator.</exception>
    public static void EnsureCanManage(Guid propertyOwnerId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin) || currentUser.UserId == propertyOwnerId)
        {
            return;
        }

        throw new ForbiddenAccessException("Only the property owner can manage this contract.");
    }
}
