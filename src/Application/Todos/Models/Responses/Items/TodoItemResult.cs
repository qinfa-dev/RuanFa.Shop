using RuanFa.FashionShop.Application.Todos.Models.Requests;

namespace RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
public record TodoItemResult : TodoItemInfo
{
    public int Id { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
