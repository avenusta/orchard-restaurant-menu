using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using RestaurantMenu.Module.Drivers;
using RestaurantMenu.Module.Handlers;
using RestaurantMenu.Module.Indexes;
using RestaurantMenu.Module.Migrations;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddContentPart<MenuItemPart>()
            .UseDisplayDriver<MenuItemPartDisplayDriver>()
            .AddHandler<MenuItemPartHandler>();

        services.AddIndexProvider<MenuItemPartIndexProvider>();
        services.AddDataMigration<MenuItemMigrations>();

        services.AddScoped<IMenuQueryService, MenuQueryService>();
    }
}
