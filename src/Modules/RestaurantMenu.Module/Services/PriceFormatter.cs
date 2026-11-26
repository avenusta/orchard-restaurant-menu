using System.Globalization;
using RestaurantMenu.Module.Settings;

namespace RestaurantMenu.Module.Services;

public static class PriceFormatter
{
    private static readonly NumberFormatInfo ForintFormat = new()
    {
        NumberGroupSeparator = " ",
        NumberDecimalSeparator = ",",
    };

    public static string Format(decimal price, string currency) =>
        currency switch
        {
            PriceCurrencies.Forint =>
                $"{decimal.Round(price, 0, MidpointRounding.AwayFromZero).ToString("#,0", ForintFormat)} Ft",
            PriceCurrencies.Euro => $"{price.ToString("0.00", CultureInfo.InvariantCulture)} €",
            _ => $"{price.ToString("0.00", CultureInfo.InvariantCulture)} {currency}",
        };
}
