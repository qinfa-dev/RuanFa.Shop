namespace RuanFa.FashionShop.Application.Accounts.Models.Requests;
public record RoleInfo
{
    public string Name { get; set; } = null!;
    public List<string>? Permissions { get; set; }
}
