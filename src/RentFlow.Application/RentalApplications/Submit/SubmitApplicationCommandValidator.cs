using FluentValidation;

namespace RentFlow.Application.RentalApplications.Submit;

/// <inheritdoc />
internal sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    /// <summary>Initializes the validation rules for <see cref="SubmitApplicationCommand"/>.</summary>
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty();

        RuleFor(x => x.Message)
            .MaximumLength(2000);
    }
}
