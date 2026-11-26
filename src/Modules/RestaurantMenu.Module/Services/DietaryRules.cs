using RestaurantMenu.Module.Models;

namespace RestaurantMenu.Module.Services;

public static class DietaryRules
{
    public static DietaryTags Normalize(DietaryTags tags) =>
        tags.HasFlag(DietaryTags.Vegan) ? tags | DietaryTags.Vegetarian : tags;
}
