using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging;

public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>;
public interface IUserQuery<TResponse> : IQuery<TResponse>, IUserRequest;


public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>;

