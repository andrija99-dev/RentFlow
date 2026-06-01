using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.RentalApplications.Accept;

/// <summary>
/// Authorizes the property owner, then accepts the application. The domain raises a
/// <c>RentalApplicationAcceptedEvent</c> that downstream features (contracts,
/// webhooks) will consume.
/// </summary>
internal sealed class AcceptApplicationCommandHandler(
    IRentalApplicationRepository applications,
    IPropertyReadService propertyReads,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<AcceptApplicationCommand>
{
    private readonly IRentalApplicationRepository _applications = applications;
    private readonly IPropertyReadService _propertyReads = propertyReads;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _applications.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(RentalApplication), request.ApplicationId);

        var ownerId = await _propertyReads.GetOwnerIdAsync(application.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), application.PropertyId);

        PropertyAuthorization.EnsureCanManage(ownerId, _currentUser);

        application.Accept();

        _applications.Update(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
