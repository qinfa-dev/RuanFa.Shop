using ErrorOr;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
using RuanFa.FashionShop.Domain.Todos.Entities;
using RuanFa.FashionShop.Domain.Todos.Enums;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Items.Create;
public record CreateTodoItemCommand : TodoItemInfo, ICommand<TodoItemResult>;

internal sealed class CreateTodoItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    : ICommandHandler<CreateTodoItemCommand, TodoItemResult>
{

    public async Task<ErrorOr<TodoItemResult>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        // Check: todo list exist
        var todo_list = await context.TodoLists
            .FirstOrDefaultAsync(m => m.Id == request.ListId, cancellationToken: cancellationToken);
        if (todo_list is null)
            return DomainErrors.TodoItem.Resource.NotFound;

        // Create: new todo item
        TodoItem todo_item = TodoItem.Create(
            listId: todo_list.Id,
            title: request.Title,
            note: request.Note,
            priority: request.Priority ?? PriorityLevel.Low,
            reminder: request.Reminder);
        todo_item.AddDomainEvent(new TodoItemCreatedEvent(todo_item));

        context.TodoItems.Add(todo_item);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<TodoItemResult>(todo_item);
    }

}
