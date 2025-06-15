using Mapster;
using RuanFa.FashionShop.Application.Todos.Lists.Create;
using RuanFa.FashionShop.Application.Todos.Lists.Update;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;

namespace RuanFa.FashionShop.Application.Todos.Models.Mappings;
internal class TodoListMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Request
        config.NewConfig<TodoListInfo, CreateTodoListCommand>();
        config.NewConfig<TodoListInfo, UpdateTodoListCommand>();

        // Response
        config.NewConfig<TodoList, TodoListResult>();
    }
}
