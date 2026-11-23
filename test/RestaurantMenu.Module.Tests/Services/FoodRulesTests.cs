using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class FoodRulesTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void IsValidSpiciness_AcceptsZeroToThree(int spiciness) =>
        Assert.True(FoodRules.IsValidSpiciness(spiciness));

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void IsValidSpiciness_RejectsValuesOutsideZeroToThree(int spiciness) =>
        Assert.False(FoodRules.IsValidSpiciness(spiciness));
}
