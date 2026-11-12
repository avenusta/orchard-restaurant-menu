using System.Globalization;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class PriceRulesTests
{
    [Theory]
    [InlineData("0.01")]
    [InlineData("1")]
    public void IsValid_AcceptsPositivePricesWithUpToTwoDecimals(string price) =>
        Assert.True(PriceRules.IsValid(decimal.Parse(price, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("0")]
    [InlineData("-0.01")]
    public void IsValid_RejectsZeroNegativeOrTooPrecisePrices(string price) =>
        Assert.False(PriceRules.IsValid(decimal.Parse(price, CultureInfo.InvariantCulture)));
}
