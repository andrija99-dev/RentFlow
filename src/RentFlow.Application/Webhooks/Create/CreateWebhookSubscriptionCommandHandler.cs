using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Security;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.Webhooks.Create;

internal sealed class CreateWebhookSubscriptionCommandHandler(
    IWebhookSubscriptionRepository subscriptions,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ISecretGenerator secretGenerator)
    : ICommandHandler<CreateWebhookSubscriptionCommand, WebhookSubscriptionCreatedResponse>
{
    private readonly IWebhookSubscriptionRepository _subscriptions = subscriptions;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ISecretGenerator _secretGenerator = secretGenerator;

    public async Task<WebhookSubscriptionCreatedResponse> Handle(
        CreateWebhookSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated owner.");

        var secret = _secretGenerator.Generate();

        var subscription = WebhookSubscription.Create(ownerId, request.TargetUrl, secret, request.EventType);

        await _subscriptions.AddAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new WebhookSubscriptionCreatedResponse(
            subscription.Id,
            subscription.OwnerId,
            subscription.TargetUrl,
            subscription.EventType,
            subscription.IsActive,
            subscription.CreatedAtUtc,
            secret);
    }
}
