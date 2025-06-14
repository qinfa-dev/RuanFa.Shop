namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
public static partial class Permission
{
    public static class Role
    {
        public const string Users = "assign:role-users";
        public const string Create = "create:role";
        public const string Update = "update:role";
        public const string Get = "get:role";
        public const string Delete = "delete:role";

        public static readonly List<string> Module = [Create, Update, Get, Delete, Users];
    }
}
