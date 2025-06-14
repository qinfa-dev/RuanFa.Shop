using FluentValidation;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;

using UserAddressValidation = DomainErrors.Common.UserAddress.Validation;

namespace RuanFa.FashionShop.Application.Abstractions.Validations;

public class UserAddressValidator : AbstractValidator<UserAddress>
{
    private readonly AddressValidator _addressValidator;

    public UserAddressValidator()
    {
        _addressValidator = new AddressValidator();

        // Include base Address validation
        RuleFor(x => x)
            .SetValidator(_addressValidator);

        // Validate UserAddress specific properties
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage(UserAddressValidation.AddressTypeRequired.Description)
            .WithErrorCode(UserAddressValidation.AddressTypeRequired.Code);

        RuleFor(x => x.IsDefault)
            .Must(x => x == true || x == false)
            .WithMessage(UserAddressValidation.DefaultAddressFlagInvalid.Description)
            .WithErrorCode(UserAddressValidation.DefaultAddressFlagInvalid.Code);
    }
}
