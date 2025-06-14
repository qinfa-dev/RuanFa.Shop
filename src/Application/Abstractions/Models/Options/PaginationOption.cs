using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using Serilog;

namespace RuanFa.FashionShop.Application.Abstractions.Models.Options;

public record PaginationOption(int? PageSize = 10, int? PageIndex = 1);

public static class PaginationOptionExtension
{
    /// <summary>
    /// Creates a paginated list with default behavior (PageSize = 10, PageIndex = 1).
    /// If pagination values are provided, uses them; otherwise uses defaults.
    /// </summary>
    public static async Task<PagedList<T>> CreateAsync<T>(
        this IQueryable<T> query,
        PaginationOption? paginationOptions,
        CancellationToken cancellationToken = default)
    {
        var sqlQuery = query.ToQueryString();
        Log.Information("Generated SQL query: {SqlQuery}", sqlQuery);

        int totalCount = await query.CountAsync(cancellationToken);

        // Use provided values or defaults (PageSize = 10, PageIndex = 1)
        int pageSize = paginationOptions?.PageSize ?? 10;
        int pageIndex = paginationOptions?.PageIndex ?? 1;

        // Ensure valid values
        if (pageSize <= 0) pageSize = 10;
        if (pageIndex <= 0) pageIndex = 1;

        var effectivePagination = new PaginationOption(pageSize, pageIndex);
        query = query.ApplyPagination(effectivePagination);

        var items = await query.ToListAsync(cancellationToken);
        Log.Information("Returning paginated list with defaults. Page {PageIndex}, PageSize {PageSize}, Total {TotalCount}",
            pageIndex, pageSize, totalCount);

        return new PagedList<T>(
            items,
            pageIndex,
            pageSize,
            totalCount
        );
    }

    /// <summary>
    /// Creates a list with all items if no valid pagination is provided.
    /// If pagination values are provided and valid, uses them; otherwise returns all items.
    /// </summary>
    public static async Task<PagedList<T>> CreateWithAllAsync<T>(
        this IQueryable<T> query,
        PaginationOption? paginationOptions,
        CancellationToken cancellationToken = default)
    {
        var sqlQuery = query.ToQueryString();
        Log.Information("Generated SQL query: {SqlQuery}", sqlQuery);

        int totalCount = await query.CountAsync(cancellationToken);

        // If valid pagination is provided, use it
        if (paginationOptions?.PageSize > 0 && paginationOptions?.PageIndex > 0)
        {
            int pageSize = paginationOptions.PageSize.Value;
            int pageIndex = paginationOptions.PageIndex.Value;

            query = query.ApplyPagination(paginationOptions);
            var items = await query.ToListAsync(cancellationToken);

            Log.Information("Returning paginated list. Page {PageIndex}, PageSize {PageSize}, Total {TotalCount}",
                pageIndex, pageSize, totalCount);

            return new PagedList<T>(
                items,
                pageIndex,
                pageSize,
                totalCount
            );
        }

        // Otherwise, return all items
        var allItems = await query.ToListAsync(cancellationToken);
        Log.Information("Returning all items without pagination. Total count: {TotalCount}", totalCount);

        return new PagedList<T>(
            allItems,
            1, // pageIndex
            totalCount, // pageSize equals totalCount
            totalCount
        );
    }

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationOption? pagination)
    {
        if (pagination == null || !pagination.PageSize.HasValue || !pagination.PageIndex.HasValue || pagination.PageSize <= 0)
        {
            return query;
        }

        int pageSize = pagination.PageSize.Value;
        int pageIndex = pagination.PageIndex.Value;

        return query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
    }

    public static int GetTotalPages(this PaginationOption? pagination, int totalCount)
    {
        if (pagination == null || !pagination.PageSize.HasValue || pagination.PageSize <= 0)
        {
            return 1;
        }

        int pageSize = pagination.PageSize.Value;
        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
