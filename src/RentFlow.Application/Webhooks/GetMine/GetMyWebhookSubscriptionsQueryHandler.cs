using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Webhooks.GetMine;

internal sealed class GetMyWebhookSubscriptionsQueryHandler(
    IWebhookSubscriptionReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetMyWebhookSubscriptionsQuery, IReadOnlyList<WebhookSubscriptionResponse>>
{
    private readonly IWebhookSubscriptionReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<WebhookSubscriptionResponse>> Handle(
        GetMyWebhookSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated owner.");

        return await _readService.GetByOwnerAsync(ownerId, cancellationToken);
    }
}
