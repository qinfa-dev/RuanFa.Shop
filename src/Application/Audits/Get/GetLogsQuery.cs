using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Abstractions.Models;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Audits.Models.Responses;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Audits.Get;

public sealed record GetLogsQuery : QueryParameters, IQuery<PagedList<ActivityLogResult>>;

internal sealed class GetLogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetLogsQuery, PagedList<ActivityLogResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PagedList<ActivityLogResult>>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var paginatedList = await _context.ActivityLogs
            .AsQueryable()
            .AsNoTracking()
            .ApplySearch(request.Search)
            .ApplySort(request.Sort)
            .ProjectToType<ActivityLogResult>(_mapper.Config)
            .CreateAsync(
                request.Pagination,
                cancellationToken);

        return paginatedList;
    }
}
