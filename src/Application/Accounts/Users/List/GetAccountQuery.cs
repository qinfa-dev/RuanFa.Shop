using ErrorOr;
using RuanFa.FashionShop.Application.Abstractions.Models;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Users.List;
public record GetAccountProfiesQuery : QueryParameters, IQuery<PagedList<AccountProfieResult>>;
internal sealed class GetUserQueryHandler(IAccountService accountService)
    : IQueryHandler<GetAccountProfiesQuery, PagedList<AccountProfieResult>>
{
    public async Task<ErrorOr<PagedList<AccountProfieResult>>> Handle(
        GetAccountProfiesQuery request,
        CancellationToken cancellationToken)
    {
        var accountResult = await accountService
         .GetAccountProfilesAsync(request, cancellationToken);

        return accountResult.IsError
            ? accountResult.Errors
            : accountResult.Value;
    }
}
