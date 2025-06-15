using ErrorOr;

using Mapster;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Abstractions.Models;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Items;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Todos.Items.Get;

public sealed record GetTodoItemQuery : QueryParameters, IQuery<PagedList<TodoItemResult>>;

internal sealed class GetTodoItemQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoItemQuery, PagedList<TodoItemResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PagedList<TodoItemResult>>> Handle(
        GetTodoItemQuery request,
        CancellationToken cancellationToken)
    {
        var paginatedList = await _context.TodoItems
            .Include(m => m.List)
            .AsQueryable()
            .AsNoTracking()
            .ApplySearch(request.Search)
            .ApplySort(request.Sort)
            .ProjectToType<TodoItemResult>(_mapper.Config)
            .CreateAsync(
                request.Pagination,
                cancellationToken);

        return paginatedList;
    }
}
