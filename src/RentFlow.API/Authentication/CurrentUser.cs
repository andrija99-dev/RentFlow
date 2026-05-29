using System.Security.Claims;
using RentFlow.Application.Abstractions.Identity;

namespace RentFlow.API.Authentication;

/// <inheritdoc />
internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    /// <inheritdoc />
    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email);

    /// <inheritdoc />
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public bool IsInRole(string role) => Principal?.IsInRole(role) ?? false;
}
