using RuanFa.FashionShop.Application.Abstractions.Models.Options;

namespace RuanFa.FashionShop.Application.Abstractions.Models;
public record QueryParameters
{
    public SearchOptions Search { get; init; } = new();
    public PaginationOption Pagination { get; init; } = new();
    public SortOption? Sort { get; init; }
}
