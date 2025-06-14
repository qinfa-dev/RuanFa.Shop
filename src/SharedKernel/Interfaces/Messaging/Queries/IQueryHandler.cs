using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>;
