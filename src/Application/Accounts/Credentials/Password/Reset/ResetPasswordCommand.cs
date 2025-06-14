using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Reset;
public record ResetPasswordCommand : ICommand<Success>
{
    public required string Email { get; init; }
    public required string ResetToken { get; init; }
    public required string NewPassword { get; init; }
}

internal sealed class ResetPasswordCommandHandler(IAccountService accountService)
: ICommandHandler<ResetPasswordCommand, Success>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<ErrorOr<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.ResetPasswordAsync(
            request.Email,
            request.ResetToken,
            request.NewPassword,
            cancellationToken);
    }
}
