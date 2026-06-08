using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Webhooks.GetById;

internal sealed class GetWebhookSubscriptionByIdQueryHandler(
    IWebhookSubscriptionReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetWebhookSubscriptionByIdQuery, WebhookSubscriptionResponse>
{
    private readonly IWebhookSubscriptionReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<WebhookSubscriptionResponse> Handle(
        GetWebhookSubscriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var subscription = await _readService.GetByIdAsync(request.SubscriptionId, cancellationToken)
            ?? throw new NotFoundException(nameof(WebhookSubscription), request.SubscriptionId);

        WebhookAuthorization.EnsureCanManage(subscription.OwnerId, _currentUser);

        return subscription;
    }
}
