using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.Settings;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class PriceFormatterTests
{
    [Theory]
    [InlineData("2490", "2 490 Ft")]
    [InlineData("590", "590 Ft")]
    [InlineData("1234567", "1 234 567 Ft")]
    [InlineData("2890.5", "2 891 Ft")]
    public void Format_ShowsForintsWithoutDecimals(string price, string expected) =>
        Assert.Equal(expected, PriceFormatter.Format(Parse(price), PriceCurrencies.Forint));

    [Theory]
    [InlineData("12.5", "12.50 €")]
    [InlineData("3", "3.00 €")]
    public void Format_ShowsEurosWithTwoDecimals(string price, string expected) =>
        Assert.Equal(expected, PriceFormatter.Format(Parse(price), PriceCurrencies.Euro));

    [Fact]
    public void Format_FallsBackToTheCurrencyCode() =>
        Assert.Equal("4.20 USD", PriceFormatter.Format(4.2m, "USD"));

    private static decimal Parse(string price) =>
        decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture);
}
