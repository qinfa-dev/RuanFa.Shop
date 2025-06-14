namespace RuanFa.FashionShop.Application.Accounts.Models.Datas;
public record RoleInfo
{
    public string Name { get; set; } = null!;
    public List<string>? Permissions { get; set; }
}
