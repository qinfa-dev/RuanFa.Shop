using RuanFa.FashionShop.Application.Accounts.Models.Datas;

namespace RuanFa.FashionShop.Application.Accounts.Models.Requests;
public record RegisterAccountInfo : UserProfileInfo
{
    public string Password { get; set; } = string.Empty;
}
