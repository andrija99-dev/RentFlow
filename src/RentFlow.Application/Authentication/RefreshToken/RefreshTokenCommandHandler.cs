using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.RefreshToken;

/// <summary>Delegates to the token service to validate and rotate the refresh token.</summary>
internal sealed class RefreshTokenCommandHandler(ITokenService tokenService)
    : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly ITokenService _tokenService = tokenService;

    public Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        _tokenService.RefreshAsync(request.RefreshToken, cancellationToken);
}
