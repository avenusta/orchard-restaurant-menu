using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Models;
using RestaurantMenu.Module.Indexes;
using RestaurantMenu.Module.Models;
using YesSql;

namespace RestaurantMenu.Module.Services;

public sealed class MenuQueryService(IContentManager contentManager, ISession session) : IMenuQueryService
{
    public async Task<IReadOnlyList<MenuSection<ContentItem>>> GetMenuAsync(MenuFilter filter)
    {
        var categories = await GetCategoriesAsync();

        var query = session.Query<ContentItem, MenuItemPartIndex>();

        if (filter.AvailableOnly)
        {
            query = query.Where(index => index.IsAvailable);
        }

        query = filter.Diet switch
        {
            DietaryTags.Vegetarian => query.Where(index => index.IsVegetarian),
            DietaryTags.Vegan => query.Where(index => index.IsVegan),
            DietaryTags.GlutenFree => query.Where(index => index.IsGlutenFree),
            _ => query,
        };

        var items = await query
            .With<ContentItemIndex>(index => index.Published)
            .ListAsync();

        return MenuGrouping.Group(categories, items.Select(ToEntry));
    }

    private async Task<IEnumerable<MenuCategory>> GetCategoriesAsync()
    {
        var taxonomy = await contentManager.GetAsync(MenuCategoryTaxonomy.ContentItemId);

        if (taxonomy is null || !taxonomy.TryGet<TaxonomyPart>(out var taxonomyPart))
        {
            return [];
        }

        return taxonomyPart.Terms.Select(term => new MenuCategory(term.ContentItemId, term.DisplayText));
    }

    private static MenuEntry<ContentItem> ToEntry(ContentItem item)
    {
        var part = item.Get<MenuItemPart>(nameof(MenuItemPart));
        var category = part.Get<TaxonomyField>(MenuItemFields.Category);

        return new MenuEntry<ContentItem>(
            item,
            category?.TermContentItemIds.FirstOrDefault(),
            part.SortOrder,
            item.DisplayText);
    }
}
