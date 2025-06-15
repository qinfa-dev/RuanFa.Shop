using ErrorOr;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Attributes;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Assign;

[LogActivity]
public sealed record AssignRoleUsersCommand(Guid RoleId, List<Guid> UserIds) : ICommand<Updated>;

internal sealed class AssignRoleUsersCommandHandler(IRoleManagementService roleManagementService)
    : ICommandHandler<AssignRoleUsersCommand, Updated>
{
    private readonly IRoleManagementService _roleManagementService = roleManagementService;

    public async Task<ErrorOr<Updated>> Handle(
        AssignRoleUsersCommand request,
        CancellationToken cancellationToken)
    {
        var assignRolesResult = await _roleManagementService
            .AssignRolesToUserAsync(request.RoleId, request.UserIds, cancellationToken);

        return assignRolesResult;
    }
}
