using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.Properties.Common;

/// <summary>
/// Authorization guard shared by the property-mutating handlers: a listing may be
/// managed only by the owner who created it or by an administrator.
/// </summary>
internal static class PropertyAuthorization
{
    /// <summary>Throws unless the current user owns the resource identified by <paramref name="ownerId"/> (or is an admin).</summary>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the owner nor an administrator.</exception>
    public static void EnsureCanManage(Guid ownerId, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin))
        {
            return;
        }

        if (currentUser.UserId is { } userId && userId == ownerId)
        {
            return;
        }

        throw new ForbiddenAccessException("You can only manage listings that you own.");
    }
}
