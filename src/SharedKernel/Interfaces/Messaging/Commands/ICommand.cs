using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Commands;

public interface IBaseCommand;
public interface ICommand<TResult> : IRequest<ErrorOr<TResult>>, IBaseCommand;
public interface ICommand : IRequest<ErrorOr<Unit>>, IBaseCommand;
