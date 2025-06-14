namespace RuanFa.FashionShop.Application.Accounts.Models.Responses;
public record RoleResult
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = null!;
    public DateTimeOffset? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}

