using ErrorOr;

using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Abstractions.Models;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Todos.Models.Responses.Lists;
using RuanFa.FashionShop.SharedKernel.Interfaces.Messaging.Queries;

namespace RuanFa.FashionShop.Application.Todos.Lists.Get;

public sealed record GetTodoListQuery : QueryParameters, IQuery<PagedList<TodoListResult>>;

internal sealed class GetTodoQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetTodoListQuery, PagedList<TodoListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PagedList<TodoListResult>>> Handle(GetTodoListQuery request, CancellationToken cancellationToken)
    {
        // Get the queryable Todos
        var paginatedList = await _context.TodoLists
          .AsQueryable()
          .AsNoTracking()
          .ApplySearch(request.Search)
          .ApplySort(request.Sort)
          .ProjectToType<TodoListResult>(_mapper.Config)
          .CreateAsync(
              request.Pagination,
              cancellationToken);

        return paginatedList;
    }
}
