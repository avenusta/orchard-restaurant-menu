using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using RestaurantMenu.Module.Indexes;
using RestaurantMenu.Module.Models;
using YesSql.Sql;

namespace RestaurantMenu.Module.Migrations;

public sealed class MenuItemMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public MenuItemMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public async Task<int> CreateAsync()
    {
        await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(MenuItemPart), part => part
            .Attachable()
            .WithDescription("Price, availability and ordering of an item on the menu."));

        await _contentDefinitionManager.AlterTypeDefinitionAsync("Food", type => type
            .Creatable()
            .Listable()
            .Draftable()
            .Versionable()
            .Securable()
            .WithPart("TitlePart", part => part.WithPosition("0"))
            .WithPart("AutoroutePart", part => part
                .WithPosition("1")
                .WithSettings(new AutoroutePartSettings
                {
                    Pattern = "menu/{{ ContentItem | display_text | slugify }}",
                }))
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
}
