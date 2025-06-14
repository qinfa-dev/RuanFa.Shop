using FluentValidation;

using EmailResendConfirmationValidation = DomainErrors.EmailManagement.ResendConfirmation.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Resend;
internal class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(EmailResendConfirmationValidation.ResendEmailRequired.Description)
            .WithErrorCode(EmailResendConfirmationValidation.ResendEmailRequired.Code)
            .EmailAddress()
            .WithMessage(EmailResendConfirmationValidation.ResendInvalidEmailFormat.Description)
            .WithErrorCode(EmailResendConfirmationValidation.ResendInvalidEmailFormat.Code);
    }
}
