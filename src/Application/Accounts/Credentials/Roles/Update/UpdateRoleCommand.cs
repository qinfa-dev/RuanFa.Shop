using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Update;
public sealed record UpdateRoleCommand : RoleInfo, ICommand<Updated>
{
    public Guid RoleId { get; set; }
}
internal sealed class UpdateRoleCommandHandler(IRoleManagementService roleManagementService)
    : ICommandHandler<UpdateRoleCommand, Updated>
{
    private readonly IRoleManagementService _roleManagementService = roleManagementService;

    public async Task<ErrorOr<Updated>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var updateRoleResult = await _roleManagementService
            .UpdateRoleAsync(request.RoleId, request, cancellationToken);

        return updateRoleResult;
    }
}
