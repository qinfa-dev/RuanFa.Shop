using ErrorOr;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Todos.Models.Requests;
using RuanFa.FashionShop.Domain.Todos.AggregateRoot;
using RuanFa.FashionShop.Domain.Todos.Events;
using RuanFa.FashionShop.Domain.Todos.ValueObjects;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Todos.Lists.Update;
public sealed record UpdateTodoListCommand : TodoListInfo, ICommand<Updated>
{
    public int ListId { get; set; }
}

internal sealed class UpdateTodoCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateTodoListCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        // Check: todo list existing
        TodoList? todo_list = await _context.TodoLists
           .SingleOrDefaultAsync(t => t.Id == request.ListId, cancellationToken);
        if (todo_list == null)
            return DomainErrors.TodoList.Resource.ListNotFound;

        // Check: duplicate title
        var duplicate_title = await _context.TodoLists
            .AnyAsync(m => m.Title == todo_list.Title && m.Id != request.ListId, cancellationToken: cancellationToken);
        if (duplicate_title)
            return DomainErrors.TodoList.Business.DuplicateTitle(todo_list.Title);

        // Check: colors
        var color_result = Colour.Create(request.ColorCode);
        if (color_result.IsError) return color_result.Errors;

        // Update: todo list
        todo_list.Title = request.Title;
        todo_list.Colour = color_result.Value;
        todo_list.AddDomainEvent(new TodoListDeletedEvent(todo_list));

        _context.TodoLists.Update(todo_list);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
