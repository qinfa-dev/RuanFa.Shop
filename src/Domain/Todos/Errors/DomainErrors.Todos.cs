using ErrorOr;

public static partial class DomainErrors
{
    // Todo Item Flow
    public static class TodoItem
    {
        public static class Validation
        {
            public static Error ItemIdRequired => Error.Validation(
                code: "TodoItem.ItemIdRequired",
                description: "The item ID is required and cannot be empty.");
            public static Error InvalidItemId => Error.Validation(
                code: "TodoItem.InvalidItemId",
                description: "The specified Item Id is invalid.");

            public static Error InvalidTitle => Error.Validation(
                code: "TodoItem.InvalidTitle",
                description: "Title must not be empty or contain only whitespace.");

            public static Error TitleTooLong => Error.Validation(
                code: "TodoItem.TitleTooLong",
                description: "Title must be at least 200 characters long.");

            public static Error TitleTooShort => Error.Validation(
                code: "TodoItem.TitleTooShort",
                description: "Title must be at least 3 characters long.");

            public static Error InvalidPriority => Error.Validation(
                code: "TodoItem.InvalidPriority",
                description: "The priority value is invalid.");

            public static Error NoteTooLong => Error.Validation(
                code: "TodoItem.NoteTooLong",
                description: "The note must not exceed 500 characters.");

            public static Error InvalidReminder=> Error.Validation(
                code: "TodoItem.InvalidReminder",
                description: "Reminder must be a future date.");

            public static Error InvalidDoneAt => Error.Validation(
                code: "TodoItem.InvalidDoneAt",
                description: "The done time is invalid.");
        }
        public static class Business
        {
            public static Error AlreadyCompleted => Error.Conflict(
                    "TodoItems.AlreadyCompleted",
                    $"The todo item is already completed.");
        }
        public static class Resource
        {
            public static Error ListNotFound => Error.NotFound(
                code: "TodoItem.ListNotFound",
                description: "The specified todo list was not found.");

            public static Error NotFound => Error.NotFound(
                code: "TodoItem.NotFound",
                description: "The specified todo item was not found.");
        }
    }

    // Todo List Flow
    public static class TodoList
    {
        public static class Validation
        {
            public static Error ListIdRequired => Error.Validation(
                code: "TodoList.ListIdRequired",
                description: "The list ID is required and cannot be empty.");
            public static Error InvalidListId => Error.Validation(
                code: "TodoList.InvalidListId",
                description: "The specified list ID is invalid.");

            public static Error TitleRequired => Error.Validation(
                code: "TodoList.TitleRequired",
                description: "The title is required and cannot be empty or whitespace.");

            public static Error TitleTooShort => Error.Validation(
                code: "TodoList.TitleTooShort",
                description: "Title must be at least 3 characters long.");

            public static Error InvalidTitleLength => Error.Validation(
                code: "TodoList.TitleTooLong",
                description: "Title must not exceed 100 characters.");

            public static Error ColourRequired => Error.Validation(
                code: "TodoList.ColourRequired",
                description: "Colour is required and must not be empty.");

            public static Error InvalidColour => Error.Validation(
                code: "TodoList.InvalidColour",
                description: "The specified colour value is invalid.");
        }


        public static class Business
        {
            public static Error DuplicateTitle(string? title) => Error.Validation(
                code: "TodoList.DuplicateTitle",
                description: $"A todo list with the title '{title}' already exists.");
        }

        public static class Resource
        {
            public static Error ListNotFound => Error.NotFound(
                code: "TodoList.ListNotFound",
                description: "The specified todo list was not found.");
        }
    }
}
