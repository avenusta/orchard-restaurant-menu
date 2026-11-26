using RestaurantMenu.Module.Models;

namespace RestaurantMenu.Module.Services;

public sealed record MenuFilter(DietaryTags Diet, bool AvailableOnly)
{
    public static readonly MenuFilter None = new(DietaryTags.None, false);

    private static readonly Dictionary<string, DietaryTags> DietsByKey = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vegetarian"] = DietaryTags.Vegetarian,
        ["vegan"] = DietaryTags.Vegan,
        ["gluten-free"] = DietaryTags.GlutenFree,
    };

    public static IReadOnlyDictionary<string, DietaryTags> Diets => DietsByKey;

    public string? DietKey => DietsByKey.FirstOrDefault(pair => pair.Value == Diet).Key;

    public static MenuFilter Parse(string? diet, string? available) =>
        new(
            diet is not null && DietsByKey.TryGetValue(diet, out var tag) ? tag : DietaryTags.None,
            bool.TryParse(available, out var availableOnly) && availableOnly);
}
