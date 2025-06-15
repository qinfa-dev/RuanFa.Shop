using ErrorOr;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Items.Delete;

public record DeleteTodoItemCommand(int TodoItemId) : ICommand<Deleted>;

internal sealed class DeleteTodoItemCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteTodoItemCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        // Find the TodoItem by TodoId
        var todoItem = await context.TodoItems
            .FirstOrDefaultAsync(m => m.Id == request.TodoItemId, cancellationToken);

        if (todoItem is null)
            return DomainErrors.TodoItem.Resource.NotFound;

        // Remove the TodoItem from the DbContext
        context.TodoItems.Remove(todoItem);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
