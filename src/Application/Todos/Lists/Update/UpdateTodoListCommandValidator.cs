using FluentValidation;
using RuanFa.FashionShop.Domain.Todos.ValueObjects;

namespace RuanFa.FashionShop.Application.Todos.Lists.Update;
internal class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    public UpdateTodoListCommandValidator()
    {
        // Id validation
        RuleFor(c => c.ListId)
            .NotEmpty()
            .WithMessage(DomainErrors.TodoList.Validation.ListIdRequired.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.ListIdRequired.Code)
            .Must(id => id > 0)
            .WithMessage(DomainErrors.TodoList.Validation.InvalidListId.Description)
            .WithErrorCode(DomainErrors.TodoList.Validation.InvalidListId.Code);

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
