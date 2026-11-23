using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Modules.Services;
using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Controllers;

public sealed class MenuController(
    IMenuQueryService menuQueryService,
    IContentItemDisplayManager contentItemDisplayManager,
    IUpdateModelAccessor updateModelAccessor,
    ISlugService slugService) : Controller
{
    [Route("menu")]
    public async Task<IActionResult> Index()
    {
        var sections = new List<MenuSectionViewModel>();

        foreach (var section in await menuQueryService.GetMenuAsync())
        {
            var items = new List<IShape>();

            foreach (var item in section.Items)
            {
                items.Add(await contentItemDisplayManager.BuildDisplayAsync(item, updateModelAccessor.ModelUpdater, "Summary"));
            }

            sections.Add(new MenuSectionViewModel
            {
                Anchor = slugService.Slugify(section.Category.Name),
                Name = section.Category.Name,
                Items = items,
            });
        }

        return View(new MenuViewModel { Sections = sections });
    }
}
