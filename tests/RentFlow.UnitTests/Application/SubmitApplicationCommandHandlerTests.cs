using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Submit;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Application;

public sealed class SubmitApplicationCommandHandlerTests
{
    private readonly Mock<IRentalApplicationRepository> _applications = new();
    private readonly Mock<IPropertyRepository> _properties = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    private readonly Guid _tenantId = Guid.CreateVersion7();
    private readonly Guid _ownerId = Guid.CreateVersion7();

    private SubmitApplicationCommandHandler CreateHandler() =>
        new(_applications.Object, _properties.Object, _unitOfWork.Object, _currentUser.Object);

    private Property AvailableProperty(Guid ownerId)
    {
        var property = Property.Create(
            "Loft",
            "desc",
            Address.Create("12 Main St", "Lisbon", "1100", "PT"),
            Money.Create(950m, "EUR"),
            ownerId);
        property.Publish();
        return property;
    }

    private void SignInAsTenant() => _currentUser.SetupGet(u => u.UserId).Returns(_tenantId);

    [Fact]
    public async Task Handle_WithAvailableProperty_CreatesApplication()
    {
        SignInAsTenant();
        var property = AvailableProperty(_ownerId);
        _properties.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _applications.Setup(r => r.HasPendingApplicationAsync(property.Id, _tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await CreateHandler().Handle(new SubmitApplicationCommand(property.Id, "hi"), CancellationToken.None);

        Assert.Equal(_tenantId, response.TenantId);
        _applications.Verify(r => r.AddAsync(It.IsAny<RentalApplication>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAnonymous_ThrowsForbidden()
    {
        _currentUser.SetupGet(u => u.UserId).Returns((Guid?)null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            CreateHandler().Handle(new SubmitApplicationCommand(Guid.CreateVersion7(), null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPropertyMissing_ThrowsNotFound()
    {
        SignInAsTenant();
        _properties.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Property?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(new SubmitApplicationCommand(Guid.CreateVersion7(), null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenApplyingToOwnProperty_ThrowsConflict()
    {
        SignInAsTenant();
        var property = AvailableProperty(_tenantId);
        _properties.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);

        await Assert.ThrowsAsync<ConflictException>(() =>
            CreateHandler().Handle(new SubmitApplicationCommand(property.Id, null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPropertyNotAvailable_ThrowsConflict()
    {
        SignInAsTenant();
        var draft = Property.Create(
            "Loft", "desc", Address.Create("12 Main St", "Lisbon", "1100", "PT"), Money.Create(950m, "EUR"), _ownerId);
        _properties.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(draft);

        await Assert.ThrowsAsync<ConflictException>(() =>
            CreateHandler().Handle(new SubmitApplicationCommand(draft.Id, null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPendingApplicationExists_ThrowsConflict()
    {
        SignInAsTenant();
        var property = AvailableProperty(_ownerId);
        _properties.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _applications.Setup(r => r.HasPendingApplicationAsync(property.Id, _tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            CreateHandler().Handle(new SubmitApplicationCommand(property.Id, null), CancellationToken.None));
    }
}
