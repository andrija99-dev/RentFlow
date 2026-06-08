using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.RentalApplications.Withdraw;

internal sealed class WithdrawApplicationCommandHandler(
    IRentalApplicationRepository applications,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<WithdrawApplicationCommand>
{
    private readonly IRentalApplicationRepository _applications = applications;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _applications.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(RentalApplication), request.ApplicationId);

        ApplicationAuthorization.EnsureIsApplicant(application.TenantId, _currentUser);

        application.Withdraw();

        _applications.Update(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
