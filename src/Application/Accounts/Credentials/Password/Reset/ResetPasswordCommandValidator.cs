using FluentValidation;
using PasswordResetValidation = DomainErrors.Authentication.Password.Reset.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Reset;
internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(PasswordResetValidation.EmailRequired.Description)
            .WithErrorCode(PasswordResetValidation.EmailRequired.Code)
            .EmailAddress()
            .WithMessage(PasswordResetValidation.InvalidEmailFormat.Description)
            .WithErrorCode(PasswordResetValidation.InvalidEmailFormat.Code);

        // Reset token validation
        RuleFor(x => x.ResetToken)
            .NotEmpty()
            .WithMessage(PasswordResetValidation.ResetTokenRequired.Description)
            .WithErrorCode(PasswordResetValidation.ResetTokenRequired.Code)
            .Length(6, 100)
            .WithMessage(PasswordResetValidation.InvalidResetTokenFormat.Description)
            .WithErrorCode(PasswordResetValidation.InvalidResetTokenFormat.Code);

        // New password validation
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(PasswordResetValidation.NewPasswordRequired.Description)
            .WithErrorCode(PasswordResetValidation.NewPasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(PasswordResetValidation.NewPasswordTooShort.Description)
            .WithErrorCode(PasswordResetValidation.NewPasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(PasswordResetValidation.InvalidPasswordFormat.Description)
            .WithErrorCode(PasswordResetValidation.InvalidPasswordFormat.Code);
    }
}
