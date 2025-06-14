using ErrorOr;
using MediatR;
using Serilog;
using Serilog.Context;

namespace RuanFa.FashionShop.Application.Abstractions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        Log.Information("Processing request {RequestName}", requestName);

        TResponse result = await next();

        if (!result.IsError)
        {
            Log.Information("Completed request {RequestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Errors, true))
            {
                Log.Error("Completed request {RequestName} with error", requestName);
            }
        }

        return result;
    }
}
