using ErrorOr;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Items.Update;
public sealed record UpdateTodoItemCommand: TodoItemInfo, ICommand<Updated>
{
    public int ItemId { get; set; }
}
internal sealed class UpdateTodoItemCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateTodoItemCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        // Check: if the TodoItem exists
        var todo_item = await _context.TodoItems
            .Include(m => m.List)
            .FirstOrDefaultAsync(t => t.Id == request.ItemId, cancellationToken);
        if (todo_item == null)
            return DomainErrors.TodoItem.Resource.NotFound;

        // Check the todo assigned is existing
        var todo_list = await _context.TodoLists
            .FirstOrDefaultAsync(t => t.Id == request.ListId, cancellationToken);
        if (todo_list == null)
            return DomainErrors.TodoItem.Resource.ListNotFound;

        // Update: the todo item
        todo_item.ListId = request.ListId;
        todo_item.Title = request.Title ?? todo_item.Title;
        todo_item.Note = request.Note ?? todo_item.Note;
        todo_item.Priority = request.Priority ?? todo_item.Priority;
        todo_item.Reminder = request.Reminder ?? todo_item.Reminder;
        todo_item.AddDomainEvent(new TodoItemUpdatedEvent(todo_item));

        _context.TodoItems.Update(todo_item);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
