using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Contracts.Common;
using RentFlow.Application.Exceptions;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.Contracts.Terminate;

internal sealed class TerminateContractCommandHandler(
    IContractRepository contracts,
    IContractReadService readService,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<TerminateContractCommand>
{
    private readonly IContractRepository _contracts = contracts;
    private readonly IContractReadService _readService = readService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(TerminateContractCommand request, CancellationToken cancellationToken)
    {
        var ownerId = await _readService.GetOwnerIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException(nameof(Contract), request.ContractId);

        ContractAuthorization.EnsureCanManage(ownerId, _currentUser);

        var contract = await _contracts.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException(nameof(Contract), request.ContractId);

        contract.Terminate();

        _contracts.Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
