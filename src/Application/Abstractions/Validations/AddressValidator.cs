using FluentValidation;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;
using AddressValidation = DomainErrors.Common.Address.Validation;

namespace RuanFa.FashionShop.Application.Abstractions.Validations;

internal class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage(AddressValidation.AddressLine1Required.Description)
            .WithErrorCode(AddressValidation.AddressLine1Required.Code);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage(AddressValidation.CityRequired.Description)
            .WithErrorCode(AddressValidation.CityRequired.Code);

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage(AddressValidation.StateRequired.Description)
            .WithErrorCode(AddressValidation.StateRequired.Code);

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage(AddressValidation.CountryRequired.Description)
            .WithErrorCode(AddressValidation.CountryRequired.Code);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage(AddressValidation.PostalCodeRequired.Description)
            .WithErrorCode(AddressValidation.PostalCodeRequired.Code)
            .Matches(@"^\d{5}(?:[-\s]\d{4})?$")
            .WithMessage(AddressValidation.InvalidPostalCodeFormat.Description)
            .WithErrorCode(AddressValidation.InvalidPostalCodeFormat.Code);
    }
}
