using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Login;

internal sealed class LoginCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService) : ICommandHandler<LoginCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

        return await _tokenService.IssueTokensAsync(user, cancellationToken);
    }
}
