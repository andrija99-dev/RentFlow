using FluentValidation;

namespace RentFlow.Application.RentalApplications.Submit;

internal sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty();

        RuleFor(x => x.Message)
            .MaximumLength(2000);
    }
}
