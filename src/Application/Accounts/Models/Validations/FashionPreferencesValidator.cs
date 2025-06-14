using FluentValidation;
using RuanFa.FashionShop.Domain.Accounts.ValueObjects;
using FashionPreferencesValidation = DomainErrors.UserProfile.FashionPreferences.Validation;
using FashionPreferencesBusiness = DomainErrors.UserProfile.FashionPreferences.Business;

namespace RuanFa.FashionShop.Application.Accounts.Models.Validations;
internal class FashionPreferencesValidator : AbstractValidator<FashionPreference>
{
    public FashionPreferencesValidator()
    {
        RuleFor(x => x.ClothingSize)
            .NotEmpty()
            .WithMessage(FashionPreferencesValidation.ClothingSizeRequired.Description)
            .WithErrorCode(FashionPreferencesValidation.ClothingSizeRequired.Code);

        RuleFor(x => x.FavoriteCategories)
            .NotEmpty()
            .WithMessage(FashionPreferencesBusiness.FavoriteCategoriesRequired.Description)
            .WithErrorCode(FashionPreferencesBusiness.FavoriteCategoriesRequired.Code)
            .Must(categories => categories.All(c => !string.IsNullOrEmpty(c)))
            .WithMessage(FashionPreferencesBusiness.InvalidCategoryName.Description)
            .WithErrorCode(FashionPreferencesBusiness.InvalidCategoryName.Code);
    }
}
