using FluentValidation;

namespace RentFlow.Application.Authentication.Login;

/// <inheritdoc />
internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Initializes the validation rules for <see cref="LoginCommand"/>.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
