using ErrorOr;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Todos.Items.GetById;

public sealed record GetTodoItemByIdQuery(int Id) : IQuery<TodoItemDetailResult>;

internal sealed class GetTodoItemByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoItemByIdQuery, TodoItemDetailResult>
{
    public async Task<ErrorOr<TodoItemDetailResult>> Handle(GetTodoItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var todo = await context.TodoItems
            .Include(m => m.List)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todo == null)
            return DomainErrors.TodoItem.Resource.NotFound;

        return mapper.Map<TodoItemDetailResult>(todo);
    }
}
