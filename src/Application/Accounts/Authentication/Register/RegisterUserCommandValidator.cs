using FluentValidation;
using RuanFa.FashionShop.Application.Abstractions.Validations;
using RuanFa.FashionShop.Application.Accounts.Models.Validations;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;

using RegisterValidation = DomainErrors.UserRegistration.Register.Validation;
using AddressValidation = DomainErrors.Common.Address.Validation;
using FashionPreferencesValidation = DomainErrors.UserProfile.FashionPreferences.Validation;

using System.Text.RegularExpressions;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.Register;

internal class RegisterCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly UserAddressValidator _userAddressValidator;

    public RegisterCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _userAddressValidator = new UserAddressValidator();

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(RegisterValidation.EmailRequired.Description)
            .WithErrorCode(RegisterValidation.EmailRequired.Code)
            .EmailAddress()
            .WithMessage(RegisterValidation.InvalidEmailFormat.Description)
            .WithErrorCode(RegisterValidation.InvalidEmailFormat.Code);

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(RegisterValidation.PasswordRequired.Description)
            .WithErrorCode(RegisterValidation.PasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(RegisterValidation.PasswordTooShort.Description)
            .WithErrorCode(RegisterValidation.PasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(RegisterValidation.InvalidPasswordFormat.Description)
            .WithErrorCode(RegisterValidation.InvalidPasswordFormat.Code);

        // FullName validation
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(RegisterValidation.FullNameRequired.Description)
            .WithErrorCode(RegisterValidation.FullNameRequired.Code)
            .MinimumLength(3)
            .WithMessage(RegisterValidation.FullNameTooShort.Description)
            .WithErrorCode(RegisterValidation.FullNameTooShort.Code);

        // PhoneNumber validation (optional)
        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () => RuleFor(x => x.PhoneNumber)
                .Must(IsValidPhoneNumber)
                .WithMessage(RegisterValidation.InvalidPhoneNumber.Description)
                .WithErrorCode(RegisterValidation.InvalidPhoneNumber.Code));

        // Gender validation
        RuleFor(x => x.Gender)
            .NotEqual(GenderType.None)
            .WithMessage(RegisterValidation.GenderRequired.Description)
            .WithErrorCode(RegisterValidation.GenderRequired.Code);

        // DateOfBirth validation (optional)
        When(x => x.DateOfBirth.HasValue, () => RuleFor(x => x.DateOfBirth)
                .Must(dob => dob!.Value < dateTimeProvider.UtcNow.AddYears(-18) && dob.Value > dateTimeProvider.UtcNow.AddYears(-120))
                .WithMessage(RegisterValidation.InvalidDateOfBirth.Description)
                .WithErrorCode(RegisterValidation.InvalidDateOfBirth.Code));

        // Addresses validation
        RuleFor(x => x.Addresses)
            .NotEmpty()
            .WithMessage(AddressValidation.AddressesRequired.Description)
            .WithErrorCode(AddressValidation.AddressesRequired.Code)
            .Must(addresses => addresses.Any(a => a.IsDefault))
            .WithMessage("At least one address must be marked as default");

        RuleForEach(x => x.Addresses)
            .SetValidator(_userAddressValidator);

        // Preferences validation (optional)
        When(x => x.Preferences != null, () => RuleFor(x => x.Preferences!)
                .SetValidator(new FashionPreferencesValidator()));

        // Wishlist validation (optional)
        When(x => x.Wishlist != null && x.Wishlist.Any(), () => RuleFor(x => x.Wishlist)
                .NotEmpty()
                .WithMessage(FashionPreferencesValidation.WishlistRequired.Description)
                .WithErrorCode(FashionPreferencesValidation.WishlistRequired.Code));
    }

    private bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Regex: Allows optional "+" at the start, followed by 10 to 15 digits
        var phoneRegex = new Regex(@"^\+?\d{10,15}$");

        return phoneRegex.IsMatch(phoneNumber);
    }
}
