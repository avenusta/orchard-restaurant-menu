namespace RestaurantMenu.Module.Services;

public static class PriceRules
{
    public const int MaxDecimalPlaces = 2;

    public static bool IsValid(decimal price) => price > 0 && decimal.Round(price, MaxDecimalPlaces) == price;
}
