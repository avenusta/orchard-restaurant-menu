namespace RestaurantMenu.Module.Services;

public sealed record MenuCategory(string Id, string Name);

public sealed record MenuEntry<TItem>(TItem Item, string? CategoryId, int SortOrder, string Title);

public sealed record MenuSection<TItem>(MenuCategory Category, IReadOnlyList<TItem> Items);

public static class MenuGrouping
{
    public static IReadOnlyList<MenuSection<TItem>> Group<TItem>(
        IEnumerable<MenuCategory> categories,
        IEnumerable<MenuEntry<TItem>> entries)
    {
        var entriesByCategory = entries
            .Where(entry => entry.CategoryId is not null)
            .ToLookup(entry => entry.CategoryId!);

        return categories
            .Select(category => new MenuSection<TItem>(
                category,
                entriesByCategory[category.Id]
                    .OrderBy(entry => entry.SortOrder)
                    .ThenBy(entry => entry.Title, StringComparer.CurrentCultureIgnoreCase)
                    .Select(entry => entry.Item)
                    .ToList()))
            .Where(section => section.Items.Count > 0)
            .ToList();
    }
}
