using FluentValidation;

namespace RuanFa.FashionShop.Application.Todos.Items.Complete;
internal sealed class CompleteTodoItemCommandValidator : AbstractValidator<CompleteTodoItemCommand>
{
    public CompleteTodoItemCommandValidator()
    {
        // Id validation
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage(DomainErrors.TodoItem.Validation.ItemIdRequired.Description)
            .WithErrorCode(DomainErrors.TodoItem.Validation.ItemIdRequired.Code)
            .Must(id => id > 0)
            .WithMessage(DomainErrors.TodoItem.Validation.InvalidItemId.Description)
            .WithErrorCode(DomainErrors.TodoItem.Validation.InvalidItemId.Code);
    }
}
