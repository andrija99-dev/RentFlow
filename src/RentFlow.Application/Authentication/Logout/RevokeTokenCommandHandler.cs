using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Logout;

/// <summary>Delegates to the token service to revoke the presented refresh token.</summary>
internal sealed class RevokeTokenCommandHandler(ITokenService tokenService)
    : ICommandHandler<RevokeTokenCommand>
{
    private readonly ITokenService _tokenService = tokenService;

    public Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken) =>
        _tokenService.RevokeAsync(request.RefreshToken, cancellationToken);
}
