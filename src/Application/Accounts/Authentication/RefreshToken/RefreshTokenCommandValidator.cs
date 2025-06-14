using FluentValidation;

using RefreshTokenValidation = DomainErrors.Authentication.RefreshToken.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.RefreshToken;
internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(RefreshTokenValidation.RefreshTokenRequired.Description)
            .WithErrorCode(RefreshTokenValidation.RefreshTokenRequired.Code)
            .Must(IsValidRefreshTokenFormat)
            .WithMessage(RefreshTokenValidation.InvalidRefreshToken.Description)
            .WithErrorCode(RefreshTokenValidation.InvalidRefreshToken.Code);
    }

    private bool IsValidRefreshTokenFormat(string refreshToken)
    {
        // Basic format validation - adjust according to your token format
        return !string.IsNullOrWhiteSpace(refreshToken) &&
               refreshToken.Length >= 32 &&
               refreshToken.Length <= 256;
    }
}
