using FluentValidation;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Webhooks.Create;

/// <inheritdoc />
internal sealed class CreateWebhookSubscriptionCommandValidator : AbstractValidator<CreateWebhookSubscriptionCommand>
{
    public CreateWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.TargetUrl)
            .NotEmpty()
            .MaximumLength(2048)
            .Must(WebhookUrl.IsHttpAbsolute)
            .WithMessage("Target URL must be an absolute http or https URL.");

        RuleFor(x => x.EventType)
            .IsInEnum();
    }
}
