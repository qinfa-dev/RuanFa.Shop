using FluentValidation;

using EmailConfirmationValidation = DomainErrors.EmailManagement.Confirmation.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Confirm;
internal class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        // UserId validation
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(EmailConfirmationValidation.UserIdRequired.Description)
            .WithErrorCode(EmailConfirmationValidation.UserIdRequired.Code)
            .Must(IsValidUserId)
            .WithMessage(EmailConfirmationValidation.InvalidUserIdFormat.Description)
            .WithErrorCode(EmailConfirmationValidation.InvalidUserIdFormat.Code);

        // Confirmation Code validation
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(EmailConfirmationValidation.ConfirmationCodeRequired.Description)
            .WithErrorCode(EmailConfirmationValidation.ConfirmationCodeRequired.Code)
            .Length(6, 100)
            .WithMessage(EmailConfirmationValidation.InvalidConfirmationCodeFormat.Description)
            .WithErrorCode(EmailConfirmationValidation.InvalidConfirmationCodeFormat.Code);

        // Changed Email validation (optional)
        When(x => !string.IsNullOrEmpty(x.ChangedEmail), () =>
        {
            RuleFor(x => x.ChangedEmail)
                .EmailAddress()
                .WithMessage(EmailConfirmationValidation.InvalidEmailFormat.Description)
                .WithErrorCode(EmailConfirmationValidation.InvalidEmailFormat.Code);
        });
    }

    private bool IsValidUserId(string userId)
    {
        // Basic GUID format validation - adjust according to your user ID format
        return Guid.TryParse(userId, out _);
    }
}
