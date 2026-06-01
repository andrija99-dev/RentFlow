using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.RentalApplications.Submit;

/// <summary>
/// Validates that the target property is open to applications and that the tenant
/// has no pending application for it, then creates and persists the application.
/// </summary>
internal sealed class SubmitApplicationCommandHandler(
    IRentalApplicationRepository applications,
    IPropertyRepository properties,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<SubmitApplicationCommand, RentalApplicationResponse>
{
    private readonly IRentalApplicationRepository _applications = applications;
    private readonly IPropertyRepository _properties = properties;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<RentalApplicationResponse> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated tenant.");

        var property = await _properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        if (property.OwnerId == tenantId)
        {
            throw new ConflictException("You cannot apply to a property you own.");
        }

        if (property.Status != PropertyStatus.Available)
        {
            throw new ConflictException("This property is not currently accepting applications.");
        }

        if (await _applications.HasPendingApplicationAsync(property.Id, tenantId, cancellationToken))
        {
            throw new ConflictException("You already have a pending application for this property.");
        }

        var application = RentalApplication.Create(property.Id, tenantId, request.Message);

        await _applications.AddAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RentalApplicationResponse(
            application.Id,
            property.Id,
            property.Title,
            property.OwnerId,
            tenantId,
            application.Status,
            application.Message,
            application.CreatedAtUtc,
            application.DecidedAtUtc);
    }
}
