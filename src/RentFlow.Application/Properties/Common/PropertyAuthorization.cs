using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Properties.Common;

/// <summary>
/// Authorization guard shared by the property-mutating handlers: a listing may be
/// managed only by the owner who created it or by an administrator.
/// </summary>
internal static class PropertyAuthorization
{
    /// <summary>Throws when the current user may not manage the given listing.</summary>
    /// <param name="property">The listing being acted upon.</param>
    /// <param name="currentUser">The accessor for the calling user.</param>
    /// <exception cref="ForbiddenAccessException">Thrown when the caller is neither the owner nor an administrator.</exception>
    public static void EnsureCanManage(Property property, ICurrentUser currentUser)
    {
        if (currentUser.IsInRole(Roles.Admin))
        {
            return;
        }

        if (currentUser.UserId is { } userId && userId == property.OwnerId)
        {
            return;
        }

        throw new ForbiddenAccessException("You can only manage listings that you own.");
    }
}
