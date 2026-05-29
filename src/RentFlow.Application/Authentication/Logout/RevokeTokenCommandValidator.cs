using FluentValidation;

namespace RentFlow.Application.Authentication.Logout;

/// <inheritdoc />
internal sealed class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    /// <summary>Initializes the validation rules for <see cref="RevokeTokenCommand"/>.</summary>
    public RevokeTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
