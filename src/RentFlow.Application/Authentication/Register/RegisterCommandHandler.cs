using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Authentication.Register;

/// <summary>Creates the account via the identity service, then issues a token pair.</summary>
internal sealed class RegisterCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService) : ICommandHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role,
            cancellationToken);

        return await _tokenService.IssueTokensAsync(user, cancellationToken);
    }
}
