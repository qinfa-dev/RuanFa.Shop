using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Users.Get;
public sealed record GetAccountInfoQuery(Guid UserId) : IQuery<AccountInfoResult>;
internal sealed class GetAccountInfoQueryHandler(
    IAccountService accountService)
        : IQueryHandler<GetAccountInfoQuery, AccountInfoResult>
{

    public async Task<ErrorOr<AccountInfoResult>> Handle(
        GetAccountInfoQuery request,
        CancellationToken cancellationToken)
    {
        var accountResult = await accountService
            .GetAccountInfoAsync(request.UserId, cancellationToken);

        return accountResult.IsError
            ? accountResult.Errors
            : accountResult.Value;
    }
}
