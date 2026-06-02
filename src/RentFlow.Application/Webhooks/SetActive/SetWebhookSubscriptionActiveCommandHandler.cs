using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.Webhooks.SetActive;

internal sealed class SetWebhookSubscriptionActiveCommandHandler(
    IWebhookSubscriptionRepository subscriptions,
    IWebhookSubscriptionReadService readService,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<SetWebhookSubscriptionActiveCommand>
{
    private readonly IWebhookSubscriptionRepository _subscriptions = subscriptions;
    private readonly IWebhookSubscriptionReadService _readService = readService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(SetWebhookSubscriptionActiveCommand request, CancellationToken cancellationToken)
    {
        var projection = await _readService.GetByIdAsync(request.SubscriptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(WebhookSubscription), request.SubscriptionId);

        WebhookAuthorization.EnsureCanManage(projection.OwnerId, _currentUser);

        var subscription = await _subscriptions.GetByIdAsync(request.SubscriptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(WebhookSubscription), request.SubscriptionId);

        if (request.IsActive)
        {
            subscription.Activate();
        }
        else
        {
            subscription.Deactivate();
        }

        _subscriptions.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
