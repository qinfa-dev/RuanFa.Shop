using RuanFa.FashionShop.Application.Todos.Models.Requests;

namespace RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
public record TodoListResult : TodoListInfo
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
