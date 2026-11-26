namespace RestaurantMenu.Module.Settings;

public static class PriceCurrencies
{
    public const string Forint = "HUF";

    public const string Euro = "EUR";

    public static readonly IReadOnlyList<string> All = [Forint, Euro];
}
