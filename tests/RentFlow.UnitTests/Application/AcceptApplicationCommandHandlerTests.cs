using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Accept;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.UnitTests.Application;

public sealed class AcceptApplicationCommandHandlerTests
{
    private readonly Mock<IRentalApplicationRepository> _applications = new();
    private readonly Mock<IPropertyReadService> _propertyReads = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    private readonly Guid _ownerId = Guid.CreateVersion7();
    private readonly Guid _propertyId = Guid.CreateVersion7();

    private AcceptApplicationCommandHandler CreateHandler() =>
        new(_applications.Object, _propertyReads.Object, _unitOfWork.Object, _currentUser.Object);

    private RentalApplication PendingApplication() =>
        RentalApplication.Create(_propertyId, Guid.CreateVersion7(), "hi");

    [Fact]
    public async Task Handle_AsOwner_AcceptsAndSaves()
    {
        var application = PendingApplication();
        _applications.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(application);
        _propertyReads.Setup(r => r.GetOwnerIdAsync(_propertyId, It.IsAny<CancellationToken>())).ReturnsAsync(_ownerId);
        _currentUser.SetupGet(u => u.UserId).Returns(_ownerId);
        _currentUser.Setup(u => u.IsInRole(It.IsAny<string>())).Returns(false);

        await CreateHandler().Handle(new AcceptApplicationCommand(application.Id), CancellationToken.None);

        Assert.Equal(ApplicationStatus.Accepted, application.Status);
        _applications.Verify(r => r.Update(application), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenApplicationMissing_ThrowsNotFound()
    {
        _applications.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplication?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(new AcceptApplicationCommand(Guid.CreateVersion7()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenNotOwner_ThrowsForbiddenAndDoesNotSave()
    {
        var application = PendingApplication();
        _applications.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(application);
        _propertyReads.Setup(r => r.GetOwnerIdAsync(_propertyId, It.IsAny<CancellationToken>())).ReturnsAsync(_ownerId);
        _currentUser.SetupGet(u => u.UserId).Returns(Guid.CreateVersion7());
        _currentUser.Setup(u => u.IsInRole(It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            CreateHandler().Handle(new AcceptApplicationCommand(application.Id), CancellationToken.None));

        Assert.Equal(ApplicationStatus.Pending, application.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
