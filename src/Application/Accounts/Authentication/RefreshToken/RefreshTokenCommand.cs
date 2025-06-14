using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.RefreshToken;
public record RefreshTokenCommand : ICommand<TokenResult>
{
    public string RefreshToken { get; init; } = null!;
}
internal class RefreshTokenCommandHandler(IAccountService accountService)
    : ICommandHandler<RefreshTokenCommand, TokenResult>
{
    public async Task<ErrorOr<TokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenResult = await accountService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        return refreshTokenResult.IsError
            ? refreshTokenResult.Errors
            : refreshTokenResult.Value;
    }
}
