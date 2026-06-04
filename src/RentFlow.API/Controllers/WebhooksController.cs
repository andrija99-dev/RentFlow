using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.API.Webhooks;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Application.Webhooks.Create;
using RentFlow.Application.Webhooks.Delete;
using RentFlow.Application.Webhooks.GetById;
using RentFlow.Application.Webhooks.GetMine;
using RentFlow.Application.Webhooks.SetActive;
using RentFlow.Application.Webhooks.UpdateUrl;

namespace RentFlow.API.Controllers;

/// <summary>
/// Webhook subscription endpoints. Owners register URLs to receive HMAC-signed HTTP
/// callbacks when the events they subscribe to occur; deliveries are driven by the
/// transactional outbox and RabbitMQ.
/// </summary>
[ApiController]
[Route("api/webhooks")]
[Produces("application/json")]
[Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
public sealed class WebhooksController(ISender sender) : ControllerBase
{
    private const string GetByIdRouteName = "GetWebhookById";

    private readonly ISender _sender = sender;

    /// <summary>Registers a webhook subscription; the signing secret is returned once, here.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookSubscriptionCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WebhookSubscriptionCreatedResponse>> Create(
        CreateWebhookSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await _sender.Send(
            new CreateWebhookSubscriptionCommand(request.TargetUrl, request.EventType),
            cancellationToken);

        return CreatedAtRoute(GetByIdRouteName, new { id = subscription.Id }, subscription);
    }

    /// <summary>Lists the webhook subscriptions registered by the current owner.</summary>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(IReadOnlyList<WebhookSubscriptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<WebhookSubscriptionResponse>>> Mine(
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetMyWebhookSubscriptionsQuery(), cancellationToken));

    /// <summary>Retrieves a single subscription owned by the caller.</summary>
    [HttpGet("{id:guid}", Name = GetByIdRouteName)]
    [ProducesResponseType(typeof(WebhookSubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookSubscriptionResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetWebhookSubscriptionByIdQuery(id), cancellationToken));

    /// <summary>Updates the delivery URL of a subscription the caller owns.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateWebhookSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateWebhookSubscriptionUrlCommand(id, request.TargetUrl), cancellationToken);

        return NoContent();
    }

    /// <summary>Resumes deliveries for a subscription.</summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new SetWebhookSubscriptionActiveCommand(id, IsActive: true), cancellationToken);

        return NoContent();
    }

    /// <summary>Suspends deliveries for a subscription.</summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new SetWebhookSubscriptionActiveCommand(id, IsActive: false), cancellationToken);

        return NoContent();
    }

    /// <summary>Deletes a subscription the caller owns.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteWebhookSubscriptionCommand(id), cancellationToken);

        return NoContent();
    }
}
