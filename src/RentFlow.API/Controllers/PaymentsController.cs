using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Payments.Common;
using RentFlow.Application.Payments.GetById;
using RentFlow.Application.Payments.GetForContract;
using RentFlow.Application.Payments.GetMine;
using RentFlow.Application.Payments.Settle;

namespace RentFlow.API.Controllers;

/// <summary>
/// Rental payment endpoints. Payments are scheduled automatically when a contract is
/// created; these endpoints let tenants and owners read them and let the tenant
/// record a payment as settled.
/// </summary>
[ApiController]
[Route("api/payments")]
[Produces("application/json")]
[Authorize]
public sealed class PaymentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Retrieves a single payment; visible to its tenant, the property owner or an admin.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetPaymentByIdQuery(id), cancellationToken));

    /// <summary>Lists the payments owed by the current tenant.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Tenant)]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> Mine(
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetMyPaymentsQuery(), cancellationToken));

    /// <summary>Lists the payments scheduled for a contract; visible to its tenant, the owner or an admin.</summary>
    [HttpGet("/api/contracts/{contractId:guid}/payments")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> ForContract(
        Guid contractId,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetPaymentsForContractQuery(contractId), cancellationToken));

    /// <summary>Records a payment as settled; only the contract's tenant (or an admin) may do so.</summary>
    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = $"{Roles.Tenant},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new MarkPaymentAsPaidCommand(id), cancellationToken);

        return NoContent();
    }
}
