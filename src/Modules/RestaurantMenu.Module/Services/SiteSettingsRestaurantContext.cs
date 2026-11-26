using OrchardCore.Settings;
using RestaurantMenu.Module.Settings;

namespace RestaurantMenu.Module.Services;

public sealed class SiteSettingsRestaurantContext(ISiteService siteService) : IRestaurantContext
{
    public async Task<RestaurantInfo> GetCurrentAsync()
    {
        var site = await siteService.GetSiteSettingsAsync();
        var settings = site.GetOrCreate<RestaurantSettings>();

        return new RestaurantInfo(
            string.IsNullOrWhiteSpace(settings.Name) ? site.SiteName : settings.Name,
            settings.Address,
            settings.OpeningHours,
            settings.Currency);
    }
}
