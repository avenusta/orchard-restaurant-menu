using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Drivers;

public sealed class MenuItemPartDisplayDriver(IStringLocalizer<MenuItemPartDisplayDriver> stringLocalizer)
    : ContentPartDisplayDriver<MenuItemPart>
{
    public override IDisplayResult Display(MenuItemPart part, BuildPartDisplayContext context) =>
        Initialize<MenuItemPartViewModel>(GetDisplayShapeType(context), viewModel => Populate(viewModel, part))
            .Location("Detail", "Content:5")
            .Location("Summary", "Content:5");

    public override IDisplayResult Edit(MenuItemPart part, BuildPartEditorContext context) =>
        Initialize<MenuItemPartViewModel>(GetEditorShapeType(context), viewModel => Populate(viewModel, part));

    public override async Task<IDisplayResult> UpdateAsync(MenuItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new MenuItemPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        if (!PriceRules.IsValid(viewModel.Price))
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(viewModel.Price)}",
                stringLocalizer["The price must be greater than zero and have at most {0} decimal places.", PriceRules.MaxDecimalPlaces]);
        }

        part.Price = viewModel.Price;
        part.IsAvailable = viewModel.IsAvailable;
        part.SortOrder = viewModel.SortOrder;

        return Edit(part, context);
    }

    private static void Populate(MenuItemPartViewModel viewModel, MenuItemPart part)
    {
        viewModel.Price = part.Price;
        viewModel.IsAvailable = part.IsAvailable;
        viewModel.SortOrder = part.SortOrder;
        viewModel.MenuItemPart = part;
    }
}
