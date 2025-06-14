using FluentValidation;

using PasswordUpdateValidation = DomainErrors.Authentication.Password.Update.Validation;
using PasswordUpdateBusiness = DomainErrors.Authentication.Password.Update.Business;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Update;
internal class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        // Old password validation
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(PasswordUpdateValidation.OldPasswordRequired.Description)
            .WithErrorCode(PasswordUpdateValidation.OldPasswordRequired.Code);

        // New password validation
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(PasswordUpdateValidation.NewPasswordRequired.Description)
            .WithErrorCode(PasswordUpdateValidation.NewPasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(PasswordUpdateValidation.NewPasswordTooShort.Description)
            .WithErrorCode(PasswordUpdateValidation.NewPasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(PasswordUpdateValidation.InvalidPasswordFormat.Description)
            .WithErrorCode(PasswordUpdateValidation.InvalidPasswordFormat.Code)
            .NotEqual(x => x.OldPassword)
            .WithMessage(PasswordUpdateBusiness.PasswordsCannotMatch.Description)
            .WithErrorCode(PasswordUpdateBusiness.PasswordsCannotMatch.Code);
    }
}
