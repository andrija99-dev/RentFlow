using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.Payments.Common;

/// <summary>
/// Authorization guards for payments. A payment may be viewed by either party to the
/// underlying contract — the tenant or the property owner — or an admin; settling it
/// is the paying tenant's action (or an admin acting on their behalf).
/// </summary>
internal static class PaymentAuthorization
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

        throw new ForbiddenAccessException("You do not have access to this payment.");
    }

    /// <summary>Throws unless the current user is the contract's tenant (or an admin).</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the tenant nor an administrator.</exception>
    public static void EnsureCanSettle(Guid tenantId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin) || currentUser.UserId == tenantId)
        {
            return;
        }

        throw new ForbiddenAccessException("Only the contract's tenant can settle this payment.");
    }
}
