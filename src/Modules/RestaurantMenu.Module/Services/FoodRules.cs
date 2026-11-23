namespace RestaurantMenu.Module.Services;

public static class FoodRules
{
    public const int MaxSpiciness = 3;

    public static bool IsValidSpiciness(int spiciness) => spiciness is >= 0 and <= MaxSpiciness;
}
