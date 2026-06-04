using FluentValidation;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Webhooks.UpdateUrl;

/// <inheritdoc />
internal sealed class UpdateWebhookSubscriptionUrlCommandValidator
    : AbstractValidator<UpdateWebhookSubscriptionUrlCommand>
{
    public UpdateWebhookSubscriptionUrlCommandValidator()
    {
        RuleFor(x => x.TargetUrl)
            .NotEmpty()
            .MaximumLength(2048)
            .Must(WebhookUrl.IsHttpAbsolute)
            .WithMessage("Target URL must be an absolute http or https URL.");
    }
}
