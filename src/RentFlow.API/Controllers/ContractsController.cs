using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Contracts.Common;
using RentFlow.Application.Contracts.GetById;
using RentFlow.Application.Contracts.GetForProperty;
using RentFlow.Application.Contracts.GetMine;
using RentFlow.Application.Contracts.Terminate;

namespace RentFlow.API.Controllers;

/// <summary>
/// Rental contract endpoints. Contracts are generated automatically when an
/// application is accepted; these endpoints let tenants and owners read them and let
/// owners terminate an active contract early.
/// </summary>
[ApiController]
[Route("api/contracts")]
[Produces("application/json")]
[Authorize]
public sealed class ContractsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Retrieves a single contract; visible to its tenant, the property owner or an admin.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetContractByIdQuery(id), cancellationToken));

    /// <summary>Lists the contracts belonging to the current tenant.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Tenant)]
    [ProducesResponseType(typeof(IReadOnlyList<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ContractResponse>>> Mine(
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetMyContractsQuery(), cancellationToken));

    /// <summary>Lists the contracts generated for a property; only the owner may view them.</summary>
    [HttpGet("/api/properties/{propertyId:guid}/contracts")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ContractResponse>>> ForProperty(
        Guid propertyId,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetContractsForPropertyQuery(propertyId), cancellationToken));

    /// <summary>Terminates an active contract before its end date; only the property owner may do so.</summary>
    [HttpPost("{id:guid}/terminate")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Terminate(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new TerminateContractCommand(id), cancellationToken);

        return NoContent();
    }
}
