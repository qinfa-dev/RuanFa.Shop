namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
public static partial class Permission
{
    public static class Log
    {
        public const string Get = "get:role";

        public static readonly List<string> Module = [Get];
    }
}
