using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class DrinkRulesTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(330)]
    public void IsValidVolume_AcceptsPositiveVolumes(int volumeMl) =>
        Assert.True(DrinkRules.IsValidVolume(volumeMl));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValidVolume_RejectsZeroAndNegativeVolumes(int volumeMl) =>
        Assert.False(DrinkRules.IsValidVolume(volumeMl));
}
