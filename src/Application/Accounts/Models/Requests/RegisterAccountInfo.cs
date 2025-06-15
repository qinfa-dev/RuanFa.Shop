namespace RuanFa.FashionShop.Application.Accounts.Models.Requests;
public record RegisterAccountInfo : UserProfileInfo
{
    public string Password { get; set; } = string.Empty;
}
