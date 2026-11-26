namespace RestaurantMenu.Module.ViewModels;

public sealed class MenuOverviewViewModel
{
    public required IReadOnlyList<MenuOverviewSection> Sections { get; init; }
}

public sealed class MenuOverviewSection
{
    public required string Name { get; init; }

    public required IReadOnlyList<MenuOverviewItem> Items { get; init; }
}

public sealed class MenuOverviewItem
{
    public required string ContentItemId { get; init; }

    public required string Title { get; init; }

    public required string ContentType { get; init; }

    public required decimal Price { get; init; }

    public required bool IsAvailable { get; init; }
}
