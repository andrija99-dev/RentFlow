using FluentValidation;

namespace RentFlow.Application.Authentication.RefreshToken;

/// <inheritdoc />
internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>Initializes the validation rules for <see cref="RefreshTokenCommand"/>.</summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
