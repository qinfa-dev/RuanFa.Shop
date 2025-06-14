namespace RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
public class PagedList<T>(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
{
    public List<T> Items { get; set; } = [.. items];
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)(pageSize > 0 ? pageSize : 1));

    public bool HasNextPage => PageIndex < TotalPages;
    public bool HasPreviousPage => PageIndex > 1;

    public PagedList<TResponse> Map<TResponse>(Func<T, TResponse> mapFunc)
    {
        var mappedItems = Items.Select(mapFunc).ToList();
        return new PagedList<TResponse>(mappedItems, PageIndex, PageSize, TotalCount);
    }
}
