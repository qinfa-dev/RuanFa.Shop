using System.Text.RegularExpressions;
using FluentValidation;

using LoginValidation = DomainErrors.Authentication.Login.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.Login;
internal class LoginWithPasswordQueryValidator : AbstractValidator<LoginWithPasswordQuery>
{
    public LoginWithPasswordQueryValidator()
    {
        // Basic non-empty validation
        RuleFor(x => x.Credential)
            .NotEmpty()
            .WithMessage(LoginValidation.UserCredentialRequired.Description)
            .WithErrorCode(LoginValidation.UserCredentialRequired.Code)
            .Must(BeEmailOrUsername)
            .WithMessage(LoginValidation.InvalidUserCredential.Description)
            .WithErrorCode(LoginValidation.InvalidUserCredential.Code)
            .When(x => !string.IsNullOrEmpty(x.Credential));

        // Email format validation (when '@' is present)
        When(x => !string.IsNullOrEmpty(x.Credential) &&
                x.Credential.Contains('@'), () => RuleFor(x => x.Credential)
                .EmailAddress()
                .WithMessage(LoginValidation.InvalidEmailFormat.Description)
                .WithErrorCode(LoginValidation.InvalidEmailFormat.Code));

        // Username format validation (when no '@')
        When(x => !x.Credential.Contains('@'), () => RuleFor(x => x.Credential)
                .Must(IsValidUsername)
                .WithMessage(LoginValidation.InvalidUsernameFormat.Description)
                .WithErrorCode(LoginValidation.InvalidUsernameFormat.Code));

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(LoginValidation.PasswordRequired.Description)
            .WithErrorCode(LoginValidation.PasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(LoginValidation.PasswordTooShort.Description)
            .WithErrorCode(LoginValidation.PasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(LoginValidation.InvalidPasswordFormat.Description)
            .WithErrorCode(LoginValidation.InvalidPasswordFormat.Code);
    }

    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) &&
               username.Length >= 3 &&
               username.Length <= 20 &&
               Regex.IsMatch(username, @"^[a-zA-Z0-9_\.]+$");
    }

    private bool BeEmailOrUsername(string identifier)
    {
        return identifier.Contains('@') || IsValidUsername(identifier);
    }
}
