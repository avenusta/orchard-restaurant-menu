using OrchardCore.DisplayManagement;

namespace RestaurantMenu.Module.ViewModels;

public sealed class MenuViewModel
{
    public required IReadOnlyList<MenuSectionViewModel> Sections { get; init; }
}

public sealed class MenuSectionViewModel
{
    public required string Anchor { get; init; }

    public required string Name { get; init; }

    public required IReadOnlyList<IShape> Items { get; init; }
}
