namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
public static partial class Permission
{
    public static readonly List<string> AdministratorMoudle = [
        .. TodoList.Module,
        .. TodoItem.Module,
        .. User.Module,
        .. Role.Module
    ];

    public static readonly List<string> UserMoudle = [
        .. TodoList.Module,
        .. TodoItem.Module
    ];
}
