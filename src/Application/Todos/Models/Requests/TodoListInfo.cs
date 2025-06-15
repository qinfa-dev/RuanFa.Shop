namespace RuanFa.FashionShop.Application.Todos.Models.Requests;
public record TodoListInfo
{
    public string Title { get; init; } = null!;
    public string ColorCode { get; init; } = null!;
}
