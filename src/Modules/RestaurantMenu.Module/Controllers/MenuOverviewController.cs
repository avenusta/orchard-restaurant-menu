using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.Notify;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Permissions;
using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Controllers;

public sealed class MenuOverviewController(
    IAuthorizationService authorizationService,
    IMenuQueryService menuQueryService,
    IContentManager contentManager,
    INotifier notifier,
    IHtmlLocalizer<MenuOverviewController> htmlLocalizer) : Controller
{
    [Admin("Restaurant/Menu", "RestaurantMenuOverview")]
    public async Task<IActionResult> Index()
    {
        if (!await authorizationService.AuthorizeAsync(User, MenuPermissions.ManageMenu))
        {
            return Forbid();
        }

        var sections = (await menuQueryService.GetMenuAsync(MenuFilter.None))
            .Select(section => new MenuOverviewSection
            {
                Name = section.Category.Name,
                Items = section.Items.Select(ToOverviewItem).ToList(),
            })
            .ToList();

        return View(new MenuOverviewViewModel { Sections = sections });
    }

    [HttpPost]
    [Admin("Restaurant/Menu/{contentItemId}/ToggleAvailability", "RestaurantMenuToggleAvailability")]
    public async Task<IActionResult> ToggleAvailability(string contentItemId)
    {
        if (!await authorizationService.AuthorizeAsync(User, MenuPermissions.ManageMenu))
        {
            return Forbid();
        }

        var item = await contentManager.GetAsync(contentItemId, VersionOptions.DraftRequired);

        if (item is null || !item.TryGet<MenuItemPart>(out var part))
        {
            return NotFound();
        }

        part.IsAvailable = !part.IsAvailable;
        item.Apply(nameof(MenuItemPart), part);

        await contentManager.PublishAsync(item);

        await notifier.SuccessAsync(part.IsAvailable
            ? htmlLocalizer["{0} is available again.", item.DisplayText]
            : htmlLocalizer["{0} is now sold out.", item.DisplayText]);

        return RedirectToAction(nameof(Index));
    }

    private static MenuOverviewItem ToOverviewItem(ContentItem item)
    {
        var part = item.Get<MenuItemPart>(nameof(MenuItemPart));

        return new MenuOverviewItem
        {
            ContentItemId = item.ContentItemId,
            Title = item.DisplayText,
            ContentType = item.ContentType,
            Price = part.Price,
            IsAvailable = part.IsAvailable,
        };
    }
}
