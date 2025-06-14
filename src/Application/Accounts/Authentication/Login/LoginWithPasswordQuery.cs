using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.Login;

public record LoginWithPasswordQuery(string Credential, string Password) : IQuery<TokenResult>;
internal sealed class LoginWithPasswordQueryHandler(IAccountService accountService)
    : IQueryHandler<LoginWithPasswordQuery, TokenResult>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<TokenResult>> Handle(
        LoginWithPasswordQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.AuthenticateAsync(
            request.Credential,
            request.Password,
            cancellationToken);

        return result.IsError
            ? result.Errors
            : result.Value;
    }
}
