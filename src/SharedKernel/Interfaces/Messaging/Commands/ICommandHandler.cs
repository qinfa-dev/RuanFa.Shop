using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, ErrorOr<Unit>>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : ICommand<TResponse>;
