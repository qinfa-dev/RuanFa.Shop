using ErrorOr;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Lists.Delete;

public sealed record DeleteTodoListCommand(int Id) : ICommand<Deleted>;

internal sealed class DeleteTodoListCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteTodoListCommand, Deleted>
{
    private readonly IApplicationDbContext _context = context;
    public async Task<ErrorOr<Deleted>> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        // Check: Todo list existence
        TodoList? todo_list = await _context.TodoLists
            .Include(m => m.Items)
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todo_list == null)
            return DomainErrors.TodoList.Resource.ListNotFound;

        todo_list.AddDomainEvent(new TodoListDeletedEvent(todo_list));

        // Delete: the todo list
        _context.TodoLists.Remove(todo_list);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
