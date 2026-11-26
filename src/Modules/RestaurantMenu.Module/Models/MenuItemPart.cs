using OrchardCore.ContentManagement;

namespace RestaurantMenu.Module.Models;

public sealed class MenuItemPart : ContentPart
{
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int SortOrder { get; set; }

    public DietaryTags DietaryTags { get; set; }
}
