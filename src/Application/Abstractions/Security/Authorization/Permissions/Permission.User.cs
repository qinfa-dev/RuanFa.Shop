﻿namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
public static partial class Permission
{
    public static class User
    {
        public const string Create = "create:user";
        public const string Update = "update:user";
        public const string Get = "get:user";
        public const string Delete = "delete:user";

        public static readonly List<string> Module = [Create, Update, Get, Delete];
    }
}
