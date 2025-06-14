using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Get;
public class GetRolesQuery : IQuery<List<RoleResult>>;

internal sealed class GetRoleQueryHandler(IRoleManagementService roleManagementService)
    : IQueryHandler<GetRolesQuery, List<RoleResult>>
{
    private readonly IRoleManagementService _roleManagementService = roleManagementService;

    public async Task<ErrorOr<List<RoleResult>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _roleManagementService.GetAllRolesAsync(cancellationToken);

        return result;
    }
}
