using RuanFa.FashionShop.Application.Accounts.Models.Requests;

namespace RuanFa.FashionShop.Application.Accounts.Models.Responses;
public record RoleDetailResult : RoleInfo
{
    public Guid Id { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
