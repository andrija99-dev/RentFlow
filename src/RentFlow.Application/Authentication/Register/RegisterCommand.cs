using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Register;

/// <summary>Registers a new user account and issues an initial token pair.</summary>
/// <param name="Email">The email address, used as the unique user name.</param>
/// <param name="Password">The plain-text password.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Role">The role to register as; <c>Admin</c> is not self-assignable.</param>
public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    AccountRole Role) : ICommand<AuthenticationResult>;
