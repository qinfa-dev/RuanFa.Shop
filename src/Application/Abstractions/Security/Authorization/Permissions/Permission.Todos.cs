namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;

public static partial class Permission
{
    public static class TodoItem
    {
        public const string Complete = "complete:todo-item";
        public const string Create = "create:todo-item";
        public const string Update = "update:todo-item";
        public const string Get = "get:todo-item";
        public const string Delete = "delete:todo-item";

        public static readonly List<string> Module = [Complete, Create, Update, Get, Delete];
    }

    public static class TodoList
    {
        public const string Create = "create:todo-list";
        public const string Update = "update:todo-list";
        public const string Get = "get:todo-list";
        public const string Delete = "delete:todo-list";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
    }
}
