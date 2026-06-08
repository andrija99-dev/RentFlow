using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Security;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Webhooks.Create;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.UnitTests.Application;

public sealed class CreateWebhookSubscriptionCommandHandlerTests
{
    private readonly Mock<IWebhookSubscriptionRepository> _subscriptions = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<ISecretGenerator> _secretGenerator = new();

    private CreateWebhookSubscriptionCommandHandler CreateHandler() =>
        new(_subscriptions.Object, _unitOfWork.Object, _currentUser.Object, _secretGenerator.Object);

    [Fact]
    public async Task Handle_GeneratesSecretAndPersists()
    {
        var ownerId = Guid.CreateVersion7();
        _currentUser.SetupGet(u => u.UserId).Returns(ownerId);
        _secretGenerator.Setup(s => s.Generate()).Returns("generated-secret");

        var response = await CreateHandler().Handle(
            new CreateWebhookSubscriptionCommand("https://example.com/hook", WebhookEventType.ApplicationAccepted),
            CancellationToken.None);

        Assert.Equal(ownerId, response.OwnerId);
        Assert.Equal("generated-secret", response.Secret);
        Assert.Equal(WebhookEventType.ApplicationAccepted, response.EventType);
        Assert.True(response.IsActive);
        _subscriptions.Verify(r => r.AddAsync(It.IsAny<WebhookSubscription>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAnonymous_ThrowsForbidden()
    {
        _currentUser.SetupGet(u => u.UserId).Returns((Guid?)null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            CreateHandler().Handle(
                new CreateWebhookSubscriptionCommand("https://example.com/hook", WebhookEventType.ApplicationAccepted),
                CancellationToken.None));
    }
}
