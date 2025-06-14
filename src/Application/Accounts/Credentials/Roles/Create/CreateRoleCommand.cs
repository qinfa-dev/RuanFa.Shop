using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Datas;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Create;
public sealed record CreateRoleCommand : RoleInfo, ICommand<RoleResult>;

internal sealed class CreateRoleCommandHandler(IRoleManagementService roleManagementService) : ICommandHandler<CreateRoleCommand, RoleResult>
{
    private readonly IRoleManagementService _roleManagementService = roleManagementService;

    public async Task<ErrorOr<RoleResult>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        ErrorOr<RoleResult> createRoleResult = await _roleManagementService.CreateRoleAsync(request, cancellationToken);

        return createRoleResult;
    }
}
