using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthenticationResult>;
