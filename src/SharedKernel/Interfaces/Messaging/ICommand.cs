using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging;

public interface IBaseCommand;

public interface ICommand : IRequest<ErrorOr<Success>>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>, IBaseCommand;

public interface IUserCommand : ICommand, IUserRequest;


public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, ErrorOr<Success>>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : ICommand<TResponse>;

public interface IUserCommand<TResponse> : ICommand<TResponse>, IUserRequest;

