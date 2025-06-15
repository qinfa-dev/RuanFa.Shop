namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
public static partial class Permission
{
    public static readonly List<string> AdministratorModule = [
        .. TodoList.Module,
        .. TodoItem.Module,
        .. User.Module,
        .. Role.Module,
        .. Log.Module
    ];

    public static readonly List<string> UserModule = [
        .. TodoList.Module,
        .. TodoItem.Module
    ];
}
