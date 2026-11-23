using OrchardCore.ContentManagement;

namespace RestaurantMenu.Module.Models;

public sealed class DrinkPart : ContentPart
{
    public int VolumeMl { get; set; }

    public bool IsAlcoholic { get; set; }
}
