using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.API.Properties;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;
using RentFlow.Application.Properties.Create;
using RentFlow.Application.Properties.Delete;
using RentFlow.Application.Properties.GetById;
using RentFlow.Application.Properties.Publish;
using RentFlow.Application.Properties.Search;
using RentFlow.Application.Properties.Unlist;
using RentFlow.Application.Properties.Update;

namespace RentFlow.API.Controllers;

/// <summary>
/// Property listing endpoints: owner-managed CRUD and lifecycle transitions, plus
/// public browse and search backed by Dapper read queries and Redis caching.
/// </summary>
[ApiController]
[Route("api/properties")]
[Produces("application/json")]
public sealed class PropertiesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>Runs a filtered, paged search over the property catalog.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<PropertySummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PropertySummaryResponse>>> Search(
        [FromQuery] SearchPropertiesRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchPropertiesQuery(
            request.SearchTerm,
            request.City,
            request.Country,
            request.Status,
            request.MinPrice,
            request.MaxPrice,
            request.OwnerId,
            request.Page,
            request.PageSize);

        return Ok(await _sender.Send(query, cancellationToken));
    }

    /// <summary>Retrieves a single property listing, including its images.</summary>
    [HttpGet("{id:guid}", Name = nameof(GetPropertyById))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyResponse>> GetPropertyById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _sender.Send(new GetPropertyByIdQuery(id), cancellationToken));

    /// <summary>Creates a new property listing owned by the current user.</summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PropertyResponse>> Create(
        CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePropertyCommand(
            request.Title,
            request.Description,
            request.Street,
            request.City,
            request.PostalCode,
            request.Country,
            request.PriceAmount,
            request.PriceCurrency);

        var property = await _sender.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetPropertyById), new { id = property.Id }, property);
    }

    /// <summary>Updates the editable details of a listing the caller owns.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePropertyCommand(
            id,
            request.Title,
            request.Description,
            request.Street,
            request.City,
            request.PostalCode,
            request.Country,
            request.PriceAmount,
            request.PriceCurrency);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>Publishes a draft listing, making it available for applications.</summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new PublishPropertyCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Withdraws a listing from the marketplace.</summary>
    [HttpPost("{id:guid}/unlist")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unlist(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new UnlistPropertyCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>Permanently deletes a listing the caller owns.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.Owner},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeletePropertyCommand(id), cancellationToken);

        return NoContent();
    }
}
