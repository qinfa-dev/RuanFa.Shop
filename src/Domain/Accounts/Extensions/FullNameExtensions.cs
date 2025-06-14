namespace RuanFa.FashionShop.Domain.Accounts.Extensions;

public static class FullNameExtensions
{
    public static string GetFirstName(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    public static string GetLastName(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[^1] : string.Empty;
    }

    public static string GetMiddleNames(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 2
            ? string.Join(" ", parts.Skip(1).Take(parts.Length - 2))
            : string.Empty;
    }
}
