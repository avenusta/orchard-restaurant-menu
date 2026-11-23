namespace RestaurantMenu.Module.Services;

public static class DrinkRules
{
    public static bool IsValidVolume(int volumeMl) => volumeMl > 0;
}
