using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Application.Properties.Update;

/// <summary>Loads the listing, authorizes the caller, applies the new details and evicts cached copies.</summary>
internal sealed class UpdatePropertyCommandHandler(
    IPropertyRepository properties,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICacheService cache) : ICommandHandler<UpdatePropertyCommand>
{
    private readonly IPropertyRepository _properties = properties;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICacheService _cache = cache;

    public async Task Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        PropertyAuthorization.EnsureCanManage(property.OwnerId, _currentUser);

        var address = Address.Create(request.Street, request.City, request.PostalCode, request.Country);
        var price = Money.Create(request.PriceAmount, request.PriceCurrency);

        property.UpdateDetails(request.Title, request.Description, address, price);

        _properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(PropertyCacheKeys.Detail(property.Id), cancellationToken);
        await _cache.IncrementVersionAsync(PropertyCacheKeys.SearchVersion, cancellationToken);
    }
}
