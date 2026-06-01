using AutoMapper;
using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Application.Properties.Create;

/// <summary>Builds the <see cref="Property"/> aggregate, persists it and returns its representation.</summary>
internal sealed class CreatePropertyCommandHandler(
    IPropertyRepository properties,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICacheService cache,
    IMapper mapper) : ICommandHandler<CreatePropertyCommand, PropertyResponse>
{
    private readonly IPropertyRepository _properties = properties;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICacheService _cache = cache;
    private readonly IMapper _mapper = mapper;

    public async Task<PropertyResponse> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated owner.");

        var address = Address.Create(request.Street, request.City, request.PostalCode, request.Country);
        var price = Money.Create(request.PriceAmount, request.PriceCurrency);

        var property = Property.Create(request.Title, request.Description, address, price, ownerId);

        await _properties.AddAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.IncrementVersionAsync(PropertyCacheKeys.SearchVersion, cancellationToken);

        return _mapper.Map<PropertyResponse>(property);
    }
}
