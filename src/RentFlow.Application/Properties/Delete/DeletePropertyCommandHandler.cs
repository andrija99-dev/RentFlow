using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.Properties.Delete;

/// <summary>Authorizes the caller, removes the listing and evicts cached copies.</summary>
internal sealed class DeletePropertyCommandHandler(
    IPropertyRepository properties,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICacheService cache) : ICommandHandler<DeletePropertyCommand>
{
    private readonly IPropertyRepository _properties = properties;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICacheService _cache = cache;

    public async Task Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        PropertyAuthorization.EnsureCanManage(property.OwnerId, _currentUser);

        _properties.Remove(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(PropertyCacheKeys.Detail(property.Id), cancellationToken);
        await _cache.IncrementVersionAsync(PropertyCacheKeys.SearchVersion, cancellationToken);
    }
}
