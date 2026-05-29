using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Login;

/// <summary>Authenticates a user with email and password and issues a token pair.</summary>
/// <param name="Email">The email address identifying the account.</param>
/// <param name="Password">The plain-text password.</param>
public sealed record LoginCommand(string Email, string Password) : ICommand<AuthenticationResult>;
