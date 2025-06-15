using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Users.Update;
public record UpdateAccountCommand : UserProfileInfo, ICommand<AccountProfieResult>
{
    public Guid UserId { get; set; }
}
internal class UpdateAccountCommandHandler(IAccountService accountService)
    : ICommandHandler<UpdateAccountCommand, AccountProfieResult>
{
    public async Task<ErrorOr<AccountProfieResult>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var createAccountResult = await accountService.UpdateAccountAsync(
            request.UserId,
            request,
            cancellationToken);

        return createAccountResult.IsError
            ? createAccountResult.Errors
            : createAccountResult.Value;
    }
}
