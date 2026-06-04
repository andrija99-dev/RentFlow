using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(ITokenService tokenService)
    : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly ITokenService _tokenService = tokenService;

    public Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        _tokenService.RefreshAsync(request.RefreshToken, cancellationToken);
}
