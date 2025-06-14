using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.SocialLogin;
public record SocialLoginQuery : IQuery<TokenResult>
{
    public string Provider { get; set; } = null!;
    public string Token { get; set; } = null!;
}

internal sealed class SocialLoginQueryHandler(IAccountService accountService)
    : IQueryHandler<SocialLoginQuery, TokenResult>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<TokenResult>> Handle(
        SocialLoginQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.SocialLoginAsync(
            request.Provider,
            request.Token,
            cancellationToken);

        return result.IsError
            ? result.Errors
            : result.Value;
    }
}
