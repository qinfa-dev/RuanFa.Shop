using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Delete;
public record DeleteRoleCommand(Guid RoleId) : ICommand<Deleted>;

internal sealed class DeleteRoleCommandHandler(IRoleManagementService roleManagementService)
    : ICommandHandler<DeleteRoleCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var deleteRoleResult = await roleManagementService.DeleteRoleAsync(request.RoleId, cancellationToken);

        return deleteRoleResult;
    }
}
