using System.Text.RegularExpressions;
using FluentValidation;
using RuanFa.FashionShop.Application.Abstractions.Validations;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;
using RuanFa.FashionShop.Application.Accounts.Models.Validations;

using CreateAccountValidation = DomainErrors.UserProfile.Validation;
using AddressValidation = DomainErrors.Common.Address.Validation;
using FashionPreferencesValidation = DomainErrors.UserProfile.FashionPreferences.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Users.Create;
internal class CreateAccountCommandValidator: AbstractValidator<CreateAccountCommand>
{
    private readonly UserAddressValidator _userAddressValidator;

    public CreateAccountCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _userAddressValidator = new UserAddressValidator();

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(CreateAccountValidation.EmailRequired.Description)
            .WithErrorCode(CreateAccountValidation.EmailRequired.Code)
            .EmailAddress()
            .WithMessage(CreateAccountValidation.InvalidEmailFormat.Description)
            .WithErrorCode(CreateAccountValidation.InvalidEmailFormat.Code);

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(CreateAccountValidation.PasswordRequired.Description)
            .WithErrorCode(CreateAccountValidation.PasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(CreateAccountValidation.PasswordTooShort.Description)
            .WithErrorCode(CreateAccountValidation.PasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(CreateAccountValidation.InvalidPasswordFormat.Description)
            .WithErrorCode(CreateAccountValidation.InvalidPasswordFormat.Code);

        // FullName validation
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(CreateAccountValidation.FullNameRequired.Description)
            .WithErrorCode(CreateAccountValidation.FullNameRequired.Code)
            .MinimumLength(3)
            .WithMessage(CreateAccountValidation.FullNameTooShort.Description)
            .WithErrorCode(CreateAccountValidation.FullNameTooShort.Code);

        // PhoneNumber validation (optional)
        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () => RuleFor(x => x.PhoneNumber)
                .Must(IsValidPhoneNumber)
                .WithMessage(CreateAccountValidation.InvalidPhoneNumber.Description)
                .WithErrorCode(CreateAccountValidation.InvalidPhoneNumber.Code));

        // Gender validation
        RuleFor(x => x.Gender)
            .NotEqual(GenderType.None)
            .WithMessage(CreateAccountValidation.GenderRequired.Description)
            .WithErrorCode(CreateAccountValidation.GenderRequired.Code);

        // DateOfBirth validation (optional)
        When(x => x.DateOfBirth.HasValue, () => RuleFor(x => x.DateOfBirth)
                .Must(dob => dob!.Value < dateTimeProvider.UtcNow.AddYears(-18) && dob.Value > dateTimeProvider.UtcNow.AddYears(-120))
                .WithMessage(CreateAccountValidation.InvalidDateOfBirth.Description)
                .WithErrorCode(CreateAccountValidation.InvalidDateOfBirth.Code));

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
