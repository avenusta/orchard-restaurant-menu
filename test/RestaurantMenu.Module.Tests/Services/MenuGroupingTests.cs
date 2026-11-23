using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Tests.Services;

public sealed class MenuGroupingTests
{
    private static readonly MenuCategory Soups = new("soups", "Soups");
    private static readonly MenuCategory Mains = new("mains", "Mains");
    private static readonly MenuCategory Desserts = new("desserts", "Desserts");

    [Fact]
    public void Group_KeepsTheCategoryOrder()
    {
        var sections = MenuGrouping.Group(
            [Mains, Soups],
            [Entry("Goulash", "soups"), Entry("Paprikash", "mains")]);

        Assert.Equal(["Mains", "Soups"], sections.Select(section => section.Category.Name));
    }

    [Fact]
    public void Group_SortsItemsBySortOrderThenTitle()
    {
        var sections = MenuGrouping.Group(
            [Mains],
            [Entry("Stuffed cabbage", "mains", 2), Entry("Paprikash", "mains", 1), Entry("Lecsó", "mains", 2)]);

        Assert.Equal(["Paprikash", "Lecsó", "Stuffed cabbage"], sections.Single().Items);
    }

    [Fact]
    public void Group_LeavesOutEmptyCategories()
    {
        var sections = MenuGrouping.Group([Soups, Desserts], [Entry("Goulash", "soups")]);

        Assert.Equal(["Soups"], sections.Select(section => section.Category.Name));
    }

    [Fact]
    public void Group_LeavesOutItemsWithoutAKnownCategory()
    {
        var sections = MenuGrouping.Group(
            [Soups],
            [Entry("Goulash", "soups"), Entry("Orphan", null), Entry("Deleted category", "removed")]);

        Assert.Equal(["Goulash"], sections.Single().Items);
    }

    private static MenuEntry<string> Entry(string title, string? categoryId, int sortOrder = 0) =>
        new(title, categoryId, sortOrder, title);
}
