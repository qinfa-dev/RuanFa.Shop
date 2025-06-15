using FluentValidation;

namespace RuanFa.FashionShop.Application.Todos.Lists.Delete;

internal sealed class DeleteTodoListCommandValidator : AbstractValidator<DeleteTodoListCommand>
{
    public DeleteTodoListCommandValidator()
    {
        // Id validation
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage(DomainErrors.TodoList.Validation.ListIdRequired.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.ListIdRequired.Code)
            .Must(id => id > 0)
            .WithMessage(DomainErrors.TodoList.Validation.InvalidListId.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.InvalidListId.Code);
    }
}
