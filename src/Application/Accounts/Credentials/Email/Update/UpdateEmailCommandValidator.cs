using FluentValidation;

using EmailUpdateValidation = DomainErrors.EmailManagement.Update.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Update;
internal class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
{
    public UpdateEmailCommandValidator()
    {

        // NewEmail validation
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .WithMessage(EmailUpdateValidation.NewEmailRequired.Description)
            .WithErrorCode(EmailUpdateValidation.NewEmailRequired.Code)
            .EmailAddress()
            .WithMessage(EmailUpdateValidation.InvalidEmailFormat.Description)
            .WithErrorCode(EmailUpdateValidation.InvalidEmailFormat.Code)
            .When(x => !string.IsNullOrEmpty(x.NewEmail));
    }
}
