using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class DietaryRulesTests
{
    [Fact]
    public void Normalize_AddsVegetarianToVegan() =>
        Assert.Equal(DietaryTags.Vegan | DietaryTags.Vegetarian, DietaryRules.Normalize(DietaryTags.Vegan));

    [Fact]
    public void Normalize_KeepsOtherTags() =>
        Assert.Equal(
            DietaryTags.Vegan | DietaryTags.Vegetarian | DietaryTags.GlutenFree,
            DietaryRules.Normalize(DietaryTags.Vegan | DietaryTags.GlutenFree));

    [Theory]
    [InlineData(DietaryTags.None)]
    [InlineData(DietaryTags.Vegetarian)]
    [InlineData(DietaryTags.GlutenFree)]
    public void Normalize_LeavesNonVeganTagsUnchanged(DietaryTags tags) =>
        Assert.Equal(tags, DietaryRules.Normalize(tags));
}
