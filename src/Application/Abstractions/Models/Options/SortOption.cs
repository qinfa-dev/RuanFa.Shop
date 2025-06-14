using System.Linq.Expressions;
using System.Reflection;

namespace RuanFa.FashionShop.Application.Abstractions.Models.Options;
public record SortOption(string? SortBy, string? SortOrder = "asc");
public static class SortOptionsExtension
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortOption? sort)
    {
        if (sort == null || string.IsNullOrEmpty(sort.SortBy))
        {
            // Default to sorting by "Id" if available; otherwise, the first property.
            var defaultProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperties().FirstOrDefault();
            if (defaultProperty == null) return query;

            sort = new SortOption(defaultProperty.Name, "asc");
        }

        // Find the property to sort by (case-insensitive)
        var propertyInfo = typeof(T).GetProperty(
            sort.SortBy!,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        );

        if (propertyInfo == null)
        {
            // Property not found; return the original query without sorting.
            return query;
        }

        // Build the expression tree: x => x.Property
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

        // Handle nullable properties
        if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // For nullable properties, use the underlying type (e.g., int? -> int)
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (underlyingType == null) return query;

            var conversion = Expression.Convert(propertyAccess, underlyingType);
            var orderByExp = Expression.Lambda(conversion, parameter);

            // Choose ascending or descending order
            return sort.SortOrder?.ToLower() == "asc"
                ? Queryable.OrderBy(query, (dynamic)orderByExp)
                : Queryable.OrderByDescending(query, (dynamic)orderByExp);
        }
        else
        {
            // For non-nullable properties, use the property type directly
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            // Choose ascending or descending order
            return sort.SortOrder?.ToLower() == "asc"
                ? Queryable.OrderBy(query, (dynamic)orderByExp)
                : Queryable.OrderByDescending(query, (dynamic)orderByExp);
        }
    }
}
