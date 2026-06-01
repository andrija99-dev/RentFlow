using FluentValidation;

namespace RentFlow.Application.Properties.Search;

/// <inheritdoc />
internal sealed class SearchPropertiesQueryValidator : AbstractValidator<SearchPropertiesQuery>
{
    /// <summary>Initializes the validation rules for <see cref="SearchPropertiesQuery"/>.</summary>
    public SearchPropertiesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .MaximumLength(100);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("'Max Price' must be greater than or equal to 'Min Price'.");
    }
}
