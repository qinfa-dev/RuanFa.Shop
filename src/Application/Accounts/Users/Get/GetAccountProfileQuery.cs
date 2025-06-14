using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Users.Get;
public sealed record GetAccountProfileQuery(Guid UserId) : IQuery<AccountProfieResult>;
internal sealed class GetAccountInfoEmailQueryHandler(
    IAccountService accountService)
        : IQueryHandler<GetAccountProfileQuery, AccountProfieResult>
{
    public async Task<ErrorOr<AccountProfieResult>> Handle(
        GetAccountProfileQuery request,
        CancellationToken cancellationToken)
    {
        var accountResult = await accountService
            .GetAccountProfileAsync(request.UserId, cancellationToken);

        return accountResult.IsError
            ? accountResult.Errors
            : accountResult.Value;
    }
}
