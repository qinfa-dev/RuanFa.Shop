using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Confirm;
public record ConfirmEmailCommand : ICommand<Success>
{
    public required string UserId { get; init; }
    public required string Code { get; init; }
    public string? ChangedEmail { get; init; }
}
public class ConfirmEmailCommandHandler(IAccountService accountService)
    : ICommandHandler<ConfirmEmailCommand, Success>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<Success>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _accountService.ConfirmEmailAsync(request.UserId,
            request.Code,
            request.ChangedEmail,
            cancellationToken);

        return result;
    }
}
