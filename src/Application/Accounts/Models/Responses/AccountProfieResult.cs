using RuanFa.FashionShop.Application.Accounts.Models.Requests;

namespace RuanFa.FashionShop.Application.Accounts.Models.Responses;
public record AccountProfieResult : UserProfileInfo
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
