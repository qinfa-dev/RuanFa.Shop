using Mapster;
using RuanFa.FashionShop.Application.Todos.Items.Create;
using RuanFa.FashionShop.Application.Todos.Items.Update;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
using RuanFa.FashionShop.Domain.Todos.Entities;

namespace RuanFa.FashionShop.Application.Todos.Models.Mappings;
internal class TodoItemMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Request
        config.NewConfig<TodoItemInfo, CreateTodoItemCommand>();
        config.NewConfig<TodoItemInfo, UpdateTodoItemCommand>();

        // Response
        config.NewConfig<TodoItem, TodoItemResult>();
    }
}
