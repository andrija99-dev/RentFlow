using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Properties.GetById;

internal sealed class GetPropertyByIdQueryHandler(
    IPropertyReadService readService,
    ICacheService cache) : IQueryHandler<GetPropertyByIdQuery, PropertyResponse>
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

    private readonly IPropertyReadService _readService = readService;
    private readonly ICacheService _cache = cache;

    public async Task<PropertyResponse> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _cache.GetOrCreateAsync(
            PropertyCacheKeys.Detail(request.PropertyId),
            ct => _readService.GetByIdAsync(request.PropertyId, ct),
            CacheLifetime,
            cancellationToken);

        return property ?? throw new NotFoundException(nameof(Property), request.PropertyId);
    }
}
