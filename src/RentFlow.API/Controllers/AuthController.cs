using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.API.Authentication;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Authentication.Login;
using RentFlow.Application.Authentication.Logout;
using RentFlow.Application.Authentication.RefreshToken;
using RentFlow.Application.Authentication.Register;

namespace RentFlow.API.Controllers;

/// <summary>
/// Authentication endpoints: registration, login, refresh-token rotation and logout.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Registers a new owner or tenant account and returns an initial token pair.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthenticationResult>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role);

        return Ok(await _sender.Send(command, cancellationToken));
    }

    /// <summary>Authenticates with email and password and returns a token pair.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResult>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        return Ok(await _sender.Send(command, cancellationToken));
    }

    /// <summary>Exchanges a valid refresh token for a new token pair (rotation).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResult>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);

        return Ok(await _sender.Send(command, cancellationToken));
    }

    /// <summary>Revokes a refresh token, ending the session (logout).</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new RevokeTokenCommand(request.RefreshToken), cancellationToken);

        return NoContent();
    }

    /// <summary>Returns the identity of the currently authenticated caller.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me([FromServices] ICurrentUser currentUser) => Ok(new
    {
        userId = currentUser.UserId,
        email = currentUser.Email,
        roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value),
    });
}
