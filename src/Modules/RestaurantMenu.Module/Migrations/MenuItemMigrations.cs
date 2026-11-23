using OrchardCore.Autoroute.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Media.Fields;
using OrchardCore.Media.Settings;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.Settings;
using OrchardCore.Title.Models;
using RestaurantMenu.Module.Indexes;
using RestaurantMenu.Module.Models;
using YesSql.Sql;

namespace RestaurantMenu.Module.Migrations;

public sealed class MenuItemMigrations(
    IContentDefinitionManager contentDefinitionManager,
    IContentManager contentManager) : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(MenuItemPart), part => part
            .Attachable()
            .WithDescription("Price, availability and ordering of an item on the menu."));

        await contentDefinitionManager.AlterTypeDefinitionAsync("Food", type => type
            .Creatable()
            .Listable()
            .Draftable()
            .Versionable()
            .Securable()
            .WithPart("TitlePart", part => part.WithPosition("0"))
            .WithPart("AutoroutePart", part => part
                .WithPosition("1")
                .WithSettings(MenuItemRouteSettings()))
            .WithPart(nameof(MenuItemPart), part => part.WithPosition("2")));

        await SchemaBuilder.CreateMapIndexTableAsync<MenuItemPartIndex>(table => table
            .Column<string>(nameof(MenuItemPartIndex.ContentItemId), column => column.WithLength(26))
            .Column<string>(nameof(MenuItemPartIndex.ContentType), column => column.WithLength(255))
            .Column<decimal>(nameof(MenuItemPartIndex.Price))
            .Column<bool>(nameof(MenuItemPartIndex.IsAvailable))
            .Column<int>(nameof(MenuItemPartIndex.SortOrder)));

        await SchemaBuilder.AlterIndexTableAsync<MenuItemPartIndex>(table => table
            .CreateIndex(
                "IDX_MenuItemPartIndex_ContentItemId",
                "DocumentId",
                nameof(MenuItemPartIndex.ContentItemId)));

        return 1;
    }

    public async Task<int> UpdateFrom1Async()
    {
        await contentDefinitionManager.AlterTypeDefinitionAsync(MenuCategoryTaxonomy.TermContentType, type => type
            .WithDisplayName("Menu category")
            .WithPart("TitlePart", part => part.WithPosition("0")));

        await CreateMenuCategoryTaxonomyAsync();

        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(MenuItemPart), part => part
            .WithField(MenuItemFields.Category, field => field
                .OfType(nameof(TaxonomyField))
                .WithDisplayName("Category")
                .WithPosition("0")
                .WithSettings(new TaxonomyFieldSettings
                {
                    TaxonomyContentItemId = MenuCategoryTaxonomy.ContentItemId,
                    Required = true,
                    Unique = true,
                }))
            .WithField(MenuItemFields.Description, field => field
                .OfType(nameof(TextField))
                .WithDisplayName("Description")
                .WithEditor("TextArea")
                .WithPosition("1"))
            .WithField(MenuItemFields.Photo, field => field
                .OfType(nameof(MediaField))
                .WithDisplayName("Photo")
                .WithPosition("2")
                .WithSettings(new MediaFieldSettings
                {
                    Multiple = false,
                })));

        return 2;
    }

    public async Task<int> UpdateFrom2Async()
    {
        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(FoodPart), part => part
            .Attachable()
            .WithDescription("How spicy a dish is."));

        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(DrinkPart), part => part
            .Attachable()
            .WithDescription("Volume of a drink and whether it contains alcohol."));

        await contentDefinitionManager.AlterTypeDefinitionAsync("Food", type => type
            .WithPart(nameof(FoodPart), part => part.WithPosition("3")));

        await contentDefinitionManager.AlterTypeDefinitionAsync("Drink", type => type
            .Creatable()
            .Listable()
            .Draftable()
            .Versionable()
            .Securable()
            .WithPart("TitlePart", part => part.WithPosition("0"))
            .WithPart("AutoroutePart", part => part
                .WithPosition("1")
                .WithSettings(MenuItemRouteSettings()))
            .WithPart(nameof(MenuItemPart), part => part.WithPosition("2"))
            .WithPart(nameof(DrinkPart), part => part.WithPosition("3")));

        return 3;
    }

    private static AutoroutePartSettings MenuItemRouteSettings() =>
        new()
        {
            Pattern = "menu/{{ ContentItem | display_text | slugify }}",
        };

    private async Task CreateMenuCategoryTaxonomyAsync()
    {
        if (await contentManager.GetAsync(MenuCategoryTaxonomy.ContentItemId) is not null)
        {
            return;
        }

        var taxonomy = await contentManager.NewAsync("Taxonomy");
        taxonomy.ContentItemId = MenuCategoryTaxonomy.ContentItemId;
        taxonomy.DisplayText = "Menu Categories";
        taxonomy.Alter<TitlePart>(part => part.Title = "Menu Categories");
        taxonomy.Alter<TaxonomyPart>(part => part.TermContentType = MenuCategoryTaxonomy.TermContentType);

        await contentManager.CreateAsync(taxonomy, VersionOptions.Published);
    }
}
