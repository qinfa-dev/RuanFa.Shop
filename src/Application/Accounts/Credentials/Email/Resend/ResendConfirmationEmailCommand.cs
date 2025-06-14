using ErrorOr;
using MediatR;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Email.Resend;
public record ResendConfirmationEmailCommand : ICommand<Success>
{
    public required string Email { get; init; }
}
internal sealed class ResendConfirmationEmailCommandHandler(IAccountService identityService)
        : IRequestHandler<ResendConfirmationEmailCommand, ErrorOr<Success>>
{
    private readonly IAccountService _accountService = identityService;

    public async Task<ErrorOr<Success>> Handle(
        ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.ResendConfirmationEmailAsync(request.Email, cancellationToken);
    }
}
