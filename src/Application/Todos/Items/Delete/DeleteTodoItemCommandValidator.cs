using FluentValidation;

namespace RuanFa.FashionShop.Application.Todos.Items.Delete;

internal sealed class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
{
    public DeleteTodoItemCommandValidator()
    {
        // Validate Id: Ensure it's not an empty GUID
        RuleFor(v => v.TodoItemId)
            .NotEmpty()
            .WithErrorCode("UpdateTodoItem.InvalidId")
            .WithMessage("The Id must be a valid non-empty GUID.");
    }
}
