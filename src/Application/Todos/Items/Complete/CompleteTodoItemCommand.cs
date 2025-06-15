using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;

namespace RuanFa.FashionShop.Application.Todos.Items.Complete;

public sealed record CompleteTodoItemCommand(int Id) : ICommand<Updated>;
internal sealed class CompleteTodoItemCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CompleteTodoItemCommand, Updated>
{
    public async Task<ErrorOr<Updated>> Handle(CompleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        // Check: Todo item existence
        var todoItem = await context.TodoItems
            .Include(m => m.List)
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (todoItem is null)
            return DomainErrors.TodoItem.Resource.NotFound;

        // Guard: check if the todo item already done
        if (todoItem.Done)
            return DomainErrors.TodoItem.Business.AlreadyCompleted;

        // Update: todo time
        todoItem.Done = true;
        todoItem.DoneAt = dateTimeProvider.UtcNow;
        todoItem.AddDomainEvent(new TodoItemCompletedEvent(todoItem));

        await context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
