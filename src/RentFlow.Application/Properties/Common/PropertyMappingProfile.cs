using AutoMapper;
using RentFlow.Domain.Entities;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Application.Properties.Common;

/// <summary>
/// AutoMapper profile translating the <see cref="Property"/> aggregate and its value
/// objects into the API response shapes. Used by the write-side handlers to return
/// the just-mutated entity; read queries project directly with Dapper instead.
/// </summary>
public sealed class PropertyMappingProfile : Profile
{
    /// <summary>Configures the property-to-DTO maps.</summary>
    public PropertyMappingProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<Money, MoneyDto>();
        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<Property, PropertyResponse>();
    }
}
