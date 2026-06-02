using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.Webhooks.Common;

/// <summary>
/// Authorization guard for webhook subscriptions. A subscription is managed only by
/// the owner who registered it (or an admin).
/// </summary>
internal static class WebhookAuthorization
{
    /// <summary>Throws unless the current user owns the subscription (or is an admin).</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the owner nor an administrator.</exception>
    public static void EnsureCanManage(Guid ownerId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin) || currentUser.UserId == ownerId)
        {
            return;
        }

        throw new ForbiddenAccessException("You do not have access to this webhook subscription.");
    }
}
