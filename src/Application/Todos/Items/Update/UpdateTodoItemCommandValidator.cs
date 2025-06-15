using FluentValidation;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;
using static DomainErrors.TodoItem.Validation;

namespace RuanFa.FashionShop.Application.Todos.Items.Update;

internal sealed class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTodoItemCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        // Validate Title: Required, not whitespace, and within length limits
        RuleFor(v => v.Title)
            .NotEmpty()
            .WithErrorCode(InvalidTitle.Code)
            .WithMessage(InvalidTitle.Description)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithErrorCode(InvalidTitle.Code)
            .WithMessage(InvalidTitle.Description)
            .MinimumLength(3)
            .WithErrorCode(TitleTooShort.Code)
            .WithMessage(TitleTooShort.Description)
            .MaximumLength(200)
            .WithErrorCode(TitleTooLong.Code)
            .WithMessage(TitleTooLong.Description);

        // Validate Note: Optional, but if provided, cannot exceed 500 characters
        RuleFor(v => v.Note)
            .MaximumLength(500)
            .WithErrorCode(NoteTooLong.Code)
            .WithMessage(NoteTooLong.Description)
            .When(v => !string.IsNullOrEmpty(v.Note));

        // Validate Priority: Optional, but if provided, Ensure it has a valid enum value
        RuleFor(v => v.Priority)
            .IsInEnum()
            .WithErrorCode(InvalidPriority.Code)
            .WithMessage(InvalidPriority.Description)
            .When(v => v.Priority.HasValue);

        // Validate Reminder: Optional, but if provided, must be in the future
        RuleFor(v => v.Reminder)
        .GreaterThanOrEqualTo(_dateTimeProvider.UtcNow)
            .WithErrorCode(InvalidReminder.Code)
            .WithMessage(InvalidReminder.Description)
            .When(v => v.Reminder.HasValue);
        _dateTimeProvider = dateTimeProvider;
    }
}
