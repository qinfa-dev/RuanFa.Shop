using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Update;
public sealed class UpdatePasswordCommand : ICommand<Updated>
{
    public string? NewPassword { get; init; }
    public string? OldPassword { get; init; }
}

public class UpdatePasswordCommandHandler(IAccountService idenittyService) : ICommand<UpdatePasswordCommand>
{
    private readonly IAccountService _idenittyService = idenittyService;

    public async Task<ErrorOr<Updated>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _idenittyService.UpdateAccountCredentialAsync(
            null,
            request.OldPassword,
            request.NewPassword,
            cancellationToken);

    }
}
