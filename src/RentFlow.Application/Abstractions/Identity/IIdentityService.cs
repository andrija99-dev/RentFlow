namespace RentFlow.Application.Abstractions.Identity;

/// <summary>
/// Abstraction over the user store and credential verification, implemented in
/// Infrastructure on top of ASP.NET Identity. Keeps the Application layer free of
/// any dependency on the concrete identity provider.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Creates a new user account, assigns it the requested role and returns the
    /// authenticated-user projection.
    /// </summary>
    /// <param name="email">The email address, used as the unique user name.</param>
    /// <param name="password">The plain-text password to hash and store.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="role">The role to grant the new account.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created user.</returns>
    /// <exception cref="Exceptions.ConflictException">Thrown when the email is already registered.</exception>
    /// <exception cref="FluentValidation.ValidationException">Thrown when the password violates the configured policy.</exception>
    Task<AuthenticatedUser> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        AccountRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the supplied credentials and returns the authenticated-user projection.
    /// </summary>
    /// <param name="email">The email address identifying the account.</param>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The authenticated user.</returns>
    /// <exception cref="Exceptions.AuthenticationException">Thrown when the credentials are invalid.</exception>
    Task<AuthenticatedUser> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
