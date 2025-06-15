using Carter;
using MediatR;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Audits.Get;
using RuanFa.FashionShop.Application.Audits.Models.Responses;
using RuanFa.FashionShop.Web.Api.Extensions;

namespace RuanFa.FashionShop.Web.Api.Audits;

public class LogModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logs")
            .WithTags("Logs")
            .WithOpenApi();

        // Get Log List
        group.MapGet("/",
            [ApiAuthorize(Permission.Log.Get)] async (
                [AsParameters] SearchOptions search,
                [AsParameters] PaginationOption pagination,
                [AsParameters] SortOption sort,
                ISender mediator) =>
            {
                var query = new GetLogsQuery
                {
                    Search = search,
                    Pagination = pagination,
                    Sort = sort
                };
                var result = await mediator.Send(query);
                return result.ToTypedResult();
            })
            .WithName("GetLogList")
            .WithDescription("Retrieves a list of logs with filtering, sorting, and pagination options.")
            .WithSummary("Get list of logs")
            .Produces<PagedList<ActivityLogResult>>()
            .ProducesValidationProblem()
            .RequireAuthorization();
    }
}
