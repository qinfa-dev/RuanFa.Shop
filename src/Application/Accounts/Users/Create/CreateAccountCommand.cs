using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Users.Create;
public record CreateAccountCommand : UserAccountInfo, ICommand<AccountProfieResult>;
internal class CreateAccountCommandHandler(IAccountService accountService)
    : ICommandHandler<CreateAccountCommand, AccountProfieResult>
{
    public async Task<ErrorOr<AccountProfieResult>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var createAccountResult = await accountService.CreateAccountAsync(
            request,
            cancellationToken);

        return createAccountResult.IsError
            ? createAccountResult.Errors
            : createAccountResult.Value;
    }
}
