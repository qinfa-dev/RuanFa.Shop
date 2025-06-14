using ErrorOr;
using MediatR;

namespace RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

public interface IBaseQuery;
public interface IQuery<TResult> : IRequest<ErrorOr<TResult>>, IBaseQuery;
