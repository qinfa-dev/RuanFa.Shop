using ErrorOr;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.Domain.Todos.ValueObjects;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Lists.Create;

public sealed record CreateTodoListCommand : TodoListInfo, ICommand<TodoListResult>;

internal sealed class CreateTodoListCommandHandler(IApplicationDbContext context, IMapper mapper) : ICommandHandler<CreateTodoListCommand, TodoListResult>
{

    public async Task<ErrorOr<TodoListResult>> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        // Check: duplciate title
        var duplicate_title = await context.TodoLists
            .AnyAsync(m => m.Title == request.Title, cancellationToken: cancellationToken);
        if (duplicate_title)
            return DomainErrors.TodoList.Business.DuplicateTitle(request.Title);

        // Check: colors
        var color_result = Colour.Create(request.ColorCode);
        if (color_result.IsError) return color_result.Errors;

        // Create: new todo list
        var todo = TodoList.Create(
            title: request.Title,
            colour: color_result.Value);

        // Raise: new todo list created
        todo.AddDomainEvent(new TodoListCreatedEvent(todo));

        context.TodoLists.Add(todo);
        await context.SaveChangesAsync(cancellationToken);

        // Map: map into result
        return mapper.Map<TodoListResult>(todo);
    }
}
