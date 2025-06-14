using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Update;
public record UpdateEmailCommand : ICommand<Updated>
{
    public string? NewEmail { get; init; }
}

internal class UpdateEmailCommandHandler(IAccountService accountService)
    : ICommandHandler<UpdateEmailCommand, Updated>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
    {
        var updateAccountResult = await accountService.UpdateAccountCredentialAsync(request.NewEmail, null, null, cancellationToken);

        return updateAccountResult.IsError
            ? updateAccountResult.Errors
            : updateAccountResult.Value;
    }
}
