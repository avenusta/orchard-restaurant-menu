using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using RestaurantMenu.Module.Drivers;
using RestaurantMenu.Module.Handlers;
using RestaurantMenu.Module.Indexes;
using RestaurantMenu.Module.Migrations;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Permissions;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddContentPart<MenuItemPart>()
            .UseDisplayDriver<MenuItemPartDisplayDriver>()
            .AddHandler<MenuItemPartHandler>();

        services.AddContentPart<FoodPart>()
            .UseDisplayDriver<FoodPartDisplayDriver>()
            .AddHandler<FoodPartHandler>();

        services.AddContentPart<DrinkPart>()
            .UseDisplayDriver<DrinkPartDisplayDriver>()
            .AddHandler<DrinkPartHandler>();

        services.AddIndexProvider<MenuItemPartIndexProvider>();
        services.AddDataMigration<MenuItemMigrations>();

        services.AddScoped<IMenuQueryService, MenuQueryService>();
        services.AddScoped<IRestaurantContext, SiteSettingsRestaurantContext>();

        services.AddSiteDisplayDriver<RestaurantSettingsDisplayDriver>();
        services.AddNavigationProvider<AdminMenu>();
        services.AddPermissionProvider<MenuPermissionProvider>();
        services.AddScoped<IAuthorizationHandler, RestaurantSettingsAuthorizationHandler>();
    }
}
