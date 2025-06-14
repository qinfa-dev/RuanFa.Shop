using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Authentication.Register;
public record RegisterUserCommand : RegisterAccountInfo, ICommand<AccountProfieResult>;
internal class RegisterCommandHandler(IAccountService accountService)
    : ICommandHandler<RegisterUserCommand, AccountProfieResult>
{
    public async Task<ErrorOr<AccountProfieResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var createAccountResult = await accountService.CreateAccountAsync(
            request,
            cancellationToken);

        return createAccountResult.IsError
            ? createAccountResult.Errors
            : createAccountResult.Value;
    }
}
