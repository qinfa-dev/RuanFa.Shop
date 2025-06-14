using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Get;
public record GetRoleByIdQuery(Guid RoleId) : IQuery<RoleDetailResult>;

internal sealed class GetRoleByIdQueryHandler(IRoleManagementService roleManagementService)
    : IQueryHandler<GetRoleByIdQuery, RoleDetailResult>
{

    public async Task<ErrorOr<RoleDetailResult>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await roleManagementService.GetRoleByIdAsync(request.RoleId, cancellationToken);

        return result;
    }
}
