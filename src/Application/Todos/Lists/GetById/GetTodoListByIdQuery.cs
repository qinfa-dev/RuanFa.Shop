using ErrorOr;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Todos.Lists.GetById;
public sealed record GetTodoListByIdQuery(int Id) : IQuery<TodoListResult>;
internal sealed class GetTodoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoListByIdQuery, TodoListResult>
{
    public async Task<ErrorOr<TodoListResult>> Handle(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        var todo_list = await context.TodoLists
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todo_list == null)
            return DomainErrors.TodoList.Resource.ListNotFound;

        var todo_list_result = mapper.Map<TodoListResult>(todo_list);

        return todo_list_result;
    }
}
