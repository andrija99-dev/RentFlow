using FluentValidation;

namespace RentFlow.Application.Properties.Create;

/// <inheritdoc />
internal sealed class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    /// <summary>Initializes the validation rules for <see cref="CreatePropertyCommand"/>.</summary>
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PriceAmount)
            .GreaterThan(0);

        RuleFor(x => x.PriceCurrency)
            .NotEmpty()
            .Length(3);
    }
}
