using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;
using RentFlow.Infrastructure.Persistence;

namespace RentFlow.Infrastructure.Identity;

/// <inheritdoc />
internal sealed class TokenService(
    RentFlowDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    JwtTokenGenerator tokenGenerator,
    IOptions<JwtOptions> options) : ITokenService
{
    private readonly RentFlowDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly JwtOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<AuthenticationResult> IssueTokensAsync(
        AuthenticatedUser user,
        CancellationToken cancellationToken = default)
    {
        var (accessToken, accessTokenExpiresAtUtc) = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = await PersistRefreshTokenAsync(user.Id, cancellationToken);

        return new AuthenticationResult(
            accessToken,
            accessTokenExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc);
    }

    /// <inheritdoc />
    public async Task<AuthenticationResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Set<RefreshToken>()
            .SingleOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            throw new AuthenticationException("The refresh token is invalid or has expired.");
        }

        var user = await _userManager.FindByIdAsync(existing.UserId.ToString())
            ?? throw new AuthenticationException("The refresh token is invalid or has expired.");

        var roles = await _userManager.GetRolesAsync(user);
        var authenticatedUser = new AuthenticatedUser(user.Id, user.Email!, [.. roles]);

        var (accessToken, accessTokenExpiresAtUtc) = _tokenGenerator.GenerateAccessToken(authenticatedUser);
        var replacement = BuildRefreshToken(user.Id);

        existing.RevokedAtUtc = DateTime.UtcNow;
        existing.ReplacedByToken = replacement.Token;

        _dbContext.Set<RefreshToken>().Add(replacement);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            accessToken,
            accessTokenExpiresAtUtc,
            replacement.Token,
            replacement.ExpiresAtUtc);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Set<RefreshToken>()
            .SingleOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            return;
        }

        existing.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<RefreshToken> PersistRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var refreshToken = BuildRefreshToken(userId);
        _dbContext.Set<RefreshToken>().Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }

    private RefreshToken BuildRefreshToken(Guid userId) => new()
    {
        UserId = userId,
        Token = _tokenGenerator.GenerateRefreshToken(),
        ExpiresAtUtc = DateTime.UtcNow.AddDays(_options.RefreshTokenDays),
    };
}
