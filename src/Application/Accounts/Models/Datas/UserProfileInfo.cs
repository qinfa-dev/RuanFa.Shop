using RuanFa.FashionShop.Domain.Accounts.ValueObjects;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;

namespace RuanFa.FashionShop.Application.Accounts.Models.Datas;
public record UserProfileInfo
{
    public string? Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public GenderType Gender { get; set; } = GenderType.None;
    public DateTimeOffset? DateOfBirth { get; set; }
    public List<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public FashionPreference Preferences { get; set; } = new FashionPreference();
    public List<string> Wishlist { get; set; } = new List<string>();
    public int LoyaltyPoints { get; set; }
    public bool MarketingConsent { get; set; }
}
