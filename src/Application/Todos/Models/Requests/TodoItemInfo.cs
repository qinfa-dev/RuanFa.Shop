using RuanFa.FashionShop.Domain.Todos.Enums;

namespace RuanFa.FashionShop.Application.Todos.Models.Requests;
public record TodoItemInfo
{
    public int ListId { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel? Priority { get; set; }
    public DateTimeOffset? Reminder { get; set; }
}
