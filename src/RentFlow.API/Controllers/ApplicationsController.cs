using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.API.Applications;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.RentalApplications.Accept;
using RentFlow.Application.RentalApplications.Common;
using RentFlow.Application.RentalApplications.GetById;
using RentFlow.Application.RentalApplications.GetForProperty;
using RentFlow.Application.RentalApplications.GetMine;
using RentFlow.Application.RentalApplications.Reject;
using RentFlow.Application.RentalApplications.Submit;
using RentFlow.Application.RentalApplications.Withdraw;

namespace RentFlow.API.Controllers;

/// <summary>
/// Rental application endpoints: tenants submit and withdraw applications and view
/// their own; owners review, accept and reject applications for their properties.
/// </summary>
[ApiController]
[Route("api/applications")]
[Produces("application/json")]
[Authorize]
public sealed class ApplicationsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Submits a rental application for a property.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Tenant)]
    [ProducesResponseType(typeof(RentalApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RentalApplicationResponse>> Submit(
        SubmitApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var application = await _sender.Send(
            new SubmitApplicationCommand(request.PropertyId, request.Message),
            cancellationToken);

        return CreatedAtRoute(nameof(GetById), new { id = application.Id }, application);
    }

    /// <summary>Retrieves a single application; visible to its tenant, the property owner or an admin.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(RentalApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RentalApplicationResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetApplicationByIdQuery(id), cancellationToken));

    /// <summary>Lists the applications submitted by the current tenant.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Tenant)]
    [ProducesResponseType(typeof(IReadOnlyList<RentalApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<RentalApplicationResponse>>> Mine(
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetMyApplicationsQuery(), cancellationToken));

    /// <summary>Lists the applications submitted for a property; only the owner may view them.</summary>
    [HttpGet("/api/properties/{propertyId:guid}/applications")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<RentalApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<RentalApplicationResponse>>> ForProperty(
        Guid propertyId,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetApplicationsForPropertyQuery(propertyId), cancellationToken));

    /// <summary>Accepts a pending application; only the property owner may do so.</summary>
    [HttpPost("{id:guid}/accept")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new AcceptApplicationCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Rejects a pending application; only the property owner may do so.</summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new RejectApplicationCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Withdraws a pending application; only the tenant who submitted it may do so.</summary>
    [HttpPost("{id:guid}/withdraw")]
    [Authorize(Roles = $"{Roles.Tenant},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new WithdrawApplicationCommand(id), cancellationToken);

        return NoContent();
    }
}
