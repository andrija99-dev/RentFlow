namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// Exposes the identity of the user making the current request, resolved from the
/// JWT claims. Used by handlers that need to attribute actions to the caller (for
/// example, stamping a new listing with its owner).
/// </summary>
public interface ICurrentUser
{
    /// <summary>Gets the authenticated user's identifier, or <see langword="null"/> when anonymous.</summary>
    Guid? UserId { get; }

    /// <summary>Gets the authenticated user's email, or <see langword="null"/> when anonymous.</summary>
    string? Email { get; }

    /// <summary>Gets a value indicating whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Determines whether the current user belongs to the given role.</summary>
    /// <param name="role">The role name to test.</param>
    /// <returns><see langword="true"/> when the user is in the role; otherwise <see langword="false"/>.</returns>
    bool IsInRole(string role);
}
