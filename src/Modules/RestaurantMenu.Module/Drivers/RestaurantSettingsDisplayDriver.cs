using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using RestaurantMenu.Module.Permissions;
using RestaurantMenu.Module.Settings;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Drivers;

public sealed class RestaurantSettingsDisplayDriver(
    IHttpContextAccessor httpContextAccessor,
    IAuthorizationService authorizationService,
    IStringLocalizer<RestaurantSettingsDisplayDriver> stringLocalizer)
    : SiteDisplayDriver<RestaurantSettings>
{
    public const string GroupId = "restaurant";

    protected override string SettingsGroupId => GroupId;

    public override async Task<IDisplayResult> EditAsync(ISite site, RestaurantSettings settings, BuildEditorContext context)
    {
        if (!await IsAuthorizedAsync())
        {
            return null!;
        }

        return Initialize<RestaurantSettingsViewModel>("RestaurantSettings_Edit", viewModel =>
            {
                viewModel.Name = settings.Name;
                viewModel.Address = settings.Address;
                viewModel.OpeningHours = settings.OpeningHours;
                viewModel.Currency = settings.Currency;
            })
            .Location("Content:1")
            .OnGroup(SettingsGroupId);
    }

    public override async Task<IDisplayResult> UpdateAsync(ISite site, RestaurantSettings settings, UpdateEditorContext context)
    {
        if (!await IsAuthorizedAsync())
        {
            return null!;
        }

        var viewModel = new RestaurantSettingsViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        if (!PriceCurrencies.All.Contains(viewModel.Currency))
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(viewModel.Currency)}",
                stringLocalizer["The currency must be one of: {0}.", string.Join(", ", PriceCurrencies.All)]);
        }

        if (context.Updater.ModelState.IsValid)
        {
            settings.Name = viewModel.Name?.Trim() ?? string.Empty;
            settings.Address = viewModel.Address?.Trim() ?? string.Empty;
            settings.OpeningHours = viewModel.OpeningHours?.Trim() ?? string.Empty;
            settings.Currency = viewModel.Currency;
        }

        return await EditAsync(site, settings, context);
    }

    private Task<bool> IsAuthorizedAsync() =>
        authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, MenuPermissions.ManageMenu);
}
