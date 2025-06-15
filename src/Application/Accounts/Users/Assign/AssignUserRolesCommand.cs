using ErrorOr;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Attributes;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Users.Assign;

[LogActivity]
public sealed record AssignUserRolesCommand(Guid UserId, List<Guid> RoleIds) : ICommand<Updated>;

internal sealed class AssignUserRolesCommandHandler(IRoleManagementService roleManagementService)
    : ICommandHandler<AssignUserRolesCommand, Updated>
{
    private readonly IRoleManagementService _roleManagementService = roleManagementService;

    public async Task<ErrorOr<Updated>> Handle(
        AssignUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var assignRolesResult = await _roleManagementService
            .AssignRolesToUserAsync(request.UserId, request.RoleIds, cancellationToken);

        return assignRolesResult;
    }
}
