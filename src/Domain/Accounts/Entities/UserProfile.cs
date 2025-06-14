using System.Text.RegularExpressions;
using ErrorOr;
using RuanFa.FashionShop.Domain.Accounts.Events;
using RuanFa.FashionShop.Domain.Accounts.ValueObjects;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;
using RuanFa.FashionShop.SharedKernel.Domains.Entities;

namespace RuanFa.FashionShop.Domain.Accounts.Entities;

public class UserProfile : AuditableEnitity<Guid>
{
    #region Properties
    public Guid? UserId { get; private set; }
    public string? Username { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public GenderType Gender { get; private set; } = GenderType.None;
    public DateTimeOffset? DateOfBirth { get; private set; }
    public List<UserAddress> Addresses { get; private set; } = new List<UserAddress>();
    public FashionPreference Preferences { get; private set; } = new FashionPreference();
    public List<string> Wishlist { get; private set; } = new List<string>();
    public int LoyaltyPoints { get; private set; }
    public bool MarketingConsent { get; private set; }
    #endregion

    #region Constructor
    private UserProfile()
    {
    }

    private UserProfile(
        Guid userId,
        string? username,
        string email,
        string fullName,
        string? phoneNumber,
        GenderType gender,
        DateTimeOffset? dateOfBirth,
        List<UserAddress> addresses,
        FashionPreference preferences,
        List<string> wishlist,
        int loyaltyPoints,
        bool marketingConsent)
    {
        UserId = userId;
        Email = email;
        Username = username;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Addresses = addresses ?? new List<UserAddress>();
        Preferences = preferences ?? new FashionPreference();
        Wishlist = wishlist ?? new List<string>();
        LoyaltyPoints = loyaltyPoints;
        MarketingConsent = marketingConsent;
    }
    #endregion

    #region Methods
    public static ErrorOr<UserProfile> Create(
    Guid? userId,
    string? username,
    string? email,
    string? fullName,
    string? phoneNumber,
    GenderType gender,
    DateTimeOffset? dateOfBirth,
    List<UserAddress>? addresses,
    FashionPreference? preferences,
    List<string>? wishlist,
    int loyaltyPoints,
    bool marketingConsent)
    {
        var errors = new List<Error>();

        if (userId == null || userId == Guid.Empty)
            errors.Add(DomainErrors.UserProfile.Validation.InvalidUserId);

        if (string.IsNullOrWhiteSpace(email))
            errors.Add(DomainErrors.UserProfile.Validation.EmailRequired);
        else if (!IsValidEmail(email))
            errors.Add(DomainErrors.UserProfile.Validation.InvalidEmailFormat);

        if (string.IsNullOrWhiteSpace(fullName))
            errors.Add(DomainErrors.UserProfile.Validation.FullNameRequired);
        else if (fullName.Length < 3)
            errors.Add(DomainErrors.UserProfile.Validation.FullNameTooShort);

        if (loyaltyPoints < 0)
            errors.Add(DomainErrors.UserProfile.Business.InvalidPoints);

        if (!string.IsNullOrEmpty(phoneNumber) && !Regex.IsMatch(phoneNumber ?? "", @"^\+?\d{10,15}$"))
            errors.Add(DomainErrors.UserProfile.Validation.InvalidPhoneNumber);

        if (errors.Any())
            return errors;

        var profile = new UserProfile(
            userId: userId!.Value,
            username: username,
            email: email!,
            fullName: fullName!,
            phoneNumber: phoneNumber,
            gender: gender,
            dateOfBirth: dateOfBirth,
            addresses: addresses ?? new List<UserAddress>(),
            preferences: preferences ?? new FashionPreference(),
            wishlist: wishlist ?? new List<string>(),
            loyaltyPoints: loyaltyPoints,
            marketingConsent: marketingConsent);

        profile.AddDomainEvent(new UserProfileCreatedEvent(profile));
        return profile;
    }

    public ErrorOr<Updated> UpdatePersonalDetails(
        string email,
        string? username,
        string fullName,
        string? phoneNumber,
        GenderType gender,
        DateTimeOffset? dateOfBirth,
        bool marketingConsent)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add(DomainErrors.UserProfile.Validation.EmailRequired);
        if (!IsValidEmail(email))
            errors.Add(DomainErrors.UserProfile.Validation.InvalidEmailFormat);

        if (string.IsNullOrWhiteSpace(fullName))
            errors.Add(DomainErrors.UserProfile.Validation.FullNameRequired);

        if (errors.Count != 0)
            return errors;

        Email = email;
        Username = username;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        MarketingConsent = marketingConsent;

        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateAddresses(List<UserAddress> addresses)
    {
        Addresses = addresses ?? new List<UserAddress>();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdatePreferences(FashionPreference preferences)
    {
        Preferences = preferences ?? new FashionPreference();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateWishlist(List<string> wishlist)
    {
        Wishlist = wishlist ?? new List<string>();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> AddLoyaltyPoints(int points)
    {
        if (points < 0)
        {
            return DomainErrors.UserProfile.Business.InvalidPoints;
        }

        LoyaltyPoints += points;
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> RedeemLoyaltyPoints(int points)
    {
        if (points < 0 || points > LoyaltyPoints)
        {
            return DomainErrors.UserProfile.Business.InvalidPoints;
        }

        LoyaltyPoints -= points;
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public Updated SetAccount(Guid? userId = null)
    {
        UserId = userId;
        return Result.Updated;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
