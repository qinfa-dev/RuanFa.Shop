using System.Linq.Expressions;
using System.Reflection;

namespace RuanFa.FashionShop.Application.Abstractions.Models.Options;
public record SearchOptions(string? Value = null);
public static class SearchExtensions
{
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, SearchOptions? search)
    {
        return search == null || string.IsNullOrEmpty(search.Value) ? query : ApplySearch(query, search.Value);
    }

    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[]? propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        // If no property selectors are provided, fallback to all public string properties.
        if (propertySelectors == null || propertySelectors.Length == 0)
        {
            var allStringProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType == typeof(string))
                .ToArray();

            if (!allStringProps.Any())
                return query;

            propertySelectors = [.. allStringProps.Select(prop =>
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, prop);
                return Expression.Lambda<Func<T, string>>(propertyAccess, parameter);
            })];
        }

        // Prepare a unified parameter for combining expressions.
        var unifiedParameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        // Get method info for ToLower() and Contains(string).
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        foreach (var selector in propertySelectors)
        {
            // Skip if the selector does not map to a string property.
            if (selector.ReturnType != typeof(string))
                continue;

            // Replace the selector's parameter with the unified parameter.
            var replacedBody = new ParameterReplacer(selector.Parameters[0], unifiedParameter)
                .Visit(selector.Body);

            // Build expression: x.Property != null && x.Property.ToLower().Contains(lowerSearchTerm)
            var notNullCheck = Expression.NotEqual(replacedBody, Expression.Constant(null, typeof(string)));
            var toLowerCall = Expression.Call(replacedBody, toLowerMethod!);
            var containsCall = Expression.Call(toLowerCall, containsMethod!, Expression.Constant(lowerSearchTerm));
            var condition = Expression.AndAlso(notNullCheck, containsCall);

            combinedExpression = combinedExpression == null
                ? condition
                : Expression.OrElse(combinedExpression, condition);
        }

        if (combinedExpression == null)
            return query;

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, unifiedParameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Applies a direct search predicate to the query.
    /// Use this method when you want to provide your own search expression (e.g. x => x.Vector.Contains("...")).
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="predicate">A predicate that defines the search condition.</param>
    /// <returns>A filtered IQueryable.</returns>
    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate)
    {
        return query.Where(predicate);
    }

    /// <summary>
    /// Helper class to replace parameters in expression trees.
    /// </summary>
    internal class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}

