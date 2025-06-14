using ErrorOr;
using MediatR;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Password.Forgot;
public record ForgotPasswordCommand : ICommand<Success>
{
    public required string Email { get; init; }
}
public class ForgotPasswordCommandHandler(IAccountService identityService)
    : IRequestHandler<ForgotPasswordCommand, ErrorOr<Success>>
{
    private readonly IAccountService _identityService = identityService;

    public async Task<ErrorOr<Success>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var forgetPasswordResult = await _identityService.ForgotPasswordAsync(request.Email, cancellationToken);

        return forgetPasswordResult.IsError
            ? forgetPasswordResult.Errors
            : forgetPasswordResult.Value;
    }
}
