using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class MenuFilterTests
{
    [Theory]
    [InlineData("vegetarian", DietaryTags.Vegetarian)]
    [InlineData("vegan", DietaryTags.Vegan)]
    [InlineData("GLUTEN-FREE", DietaryTags.GlutenFree)]
    public void Parse_ReadsKnownDiets(string diet, DietaryTags expected) =>
        Assert.Equal(expected, MenuFilter.Parse(diet, null).Diet);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("carnivore")]
    public void Parse_IgnoresMissingOrUnknownDiets(string? diet) =>
        Assert.Equal(DietaryTags.None, MenuFilter.Parse(diet, null).Diet);

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("false", false)]
    [InlineData("yes", false)]
    [InlineData(null, false)]
    public void Parse_ReadsAvailableOnly(string? available, bool expected) =>
        Assert.Equal(expected, MenuFilter.Parse(null, available).AvailableOnly);

    [Fact]
    public void DietKey_RoundTripsTheParsedDiet() =>
        Assert.Equal("gluten-free", MenuFilter.Parse("gluten-free", null).DietKey);
}
