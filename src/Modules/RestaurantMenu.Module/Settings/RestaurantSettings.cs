namespace RestaurantMenu.Module.Settings;

public sealed class RestaurantSettings
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string OpeningHours { get; set; } = string.Empty;

    public string Currency { get; set; } = PriceCurrencies.Forint;
}
