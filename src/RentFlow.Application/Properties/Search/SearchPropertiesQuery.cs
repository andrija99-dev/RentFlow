using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Search;

public sealed record SearchPropertiesQuery(
    string? SearchTerm,
    string? City,
    string? Country,
    PropertyStatus? Status,
    decimal? MinPrice,
    decimal? MaxPrice,
    Guid? OwnerId,
    int Page,
    int PageSize) : IQuery<PagedResult<PropertySummaryResponse>>;
