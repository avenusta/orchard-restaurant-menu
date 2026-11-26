using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using RestaurantMenu.Module.Drivers;
using RestaurantMenu.Module.Permissions;

namespace RestaurantMenu.Module;

public sealed class AdminMenu(IStringLocalizer<AdminMenu> stringLocalizer) : AdminNavigationProvider
{
    private static readonly RouteValueDictionary SettingsRouteValues = new()
    {
        { "area", "OrchardCore.Settings" },
        { "groupId", RestaurantSettingsDisplayDriver.GroupId },
    };

    protected override ValueTask BuildAsync(NavigationBuilder builder)
    {
        builder.Add(stringLocalizer["Restaurant"], "1", restaurant => restaurant
            .AddClass("restaurant")
            .Id("restaurant")
            .Add(stringLocalizer["Menu overview"], "1", overview => overview
                .Action("Index", "MenuOverview", new { area = "RestaurantMenu.Module" })
                .Permission(MenuPermissions.ManageMenu)
                .LocalNav())
            .Add(stringLocalizer["Settings"], "2", settings => settings
                .Action("Index", "Admin", SettingsRouteValues)
                .Permission(MenuPermissions.ManageMenu)
                .LocalNav()));

        return ValueTask.CompletedTask;
    }
}
