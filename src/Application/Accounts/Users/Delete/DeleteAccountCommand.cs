using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Users.Delete;
public record DeleteAccountCommand(Guid UserId) : ICommand<Deleted>;

internal class DeleteAccountCommandHandler(IAccountService accountService)
    : ICommandHandler<DeleteAccountCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var createAccountResult = await accountService.DeleteAccountAsync(
            request.UserId,
            cancellationToken);

        return createAccountResult.IsError
            ? createAccountResult.Errors
            : createAccountResult.Value;
    }
}
