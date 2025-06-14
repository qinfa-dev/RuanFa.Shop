using FluentValidation;

using PasswordRecoveryValidation = DomainErrors.Authentication.Password.Recovery.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Forgot;
internal class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(PasswordRecoveryValidation.EmailRequired.Description)
            .WithErrorCode(PasswordRecoveryValidation.EmailRequired.Code)
            .EmailAddress()
            .WithMessage(PasswordRecoveryValidation.InvalidEmailFormat.Description)
            .WithErrorCode(PasswordRecoveryValidation.InvalidEmailFormat.Code);
    }
}
