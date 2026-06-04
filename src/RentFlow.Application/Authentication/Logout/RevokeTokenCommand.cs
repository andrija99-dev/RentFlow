using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Logout;

public sealed record RevokeTokenCommand(string RefreshToken) : ICommand;
