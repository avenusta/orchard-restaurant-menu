namespace RestaurantMenu.Module.Models;

[Flags]
public enum DietaryTags
{
    None = 0,
    Vegetarian = 1,
    Vegan = 2,
    GlutenFree = 4,
}
