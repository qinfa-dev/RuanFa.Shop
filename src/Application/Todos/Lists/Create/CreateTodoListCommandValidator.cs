using FluentValidation;
using RuanFa.FashionShop.Application.Todos.Lists.Create;
using RuanFa.FashionShop.Domain.Todos.ValueObjects;

internal sealed class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListCommandValidator()
    {
        // Title validation
        RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage(DomainErrors.TodoList.Validation.TitleRequired.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.TitleRequired.Code)
            .MinimumLength(3)
            .WithMessage(DomainErrors.TodoList.Validation.TitleTooShort.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.TitleTooShort.Code)
            .MaximumLength(100)
            .WithMessage(DomainErrors.TodoList.Validation.InvalidTitleLength.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.InvalidTitleLength.Code);

        // ColorCode validation
        RuleFor(c => c.ColorCode)
            .NotEmpty()
            .WithMessage(DomainErrors.TodoList.Validation.ColourRequired.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.ColourRequired.Code)
            .Must(ColorExtentions.IsSupportedColor)
            .WithMessage(DomainErrors.TodoList.Validation.InvalidColour.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.InvalidColour.Code);
    }
}
