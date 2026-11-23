using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Drivers;

public sealed class FoodPartDisplayDriver(IStringLocalizer<FoodPartDisplayDriver> stringLocalizer)
    : ContentPartDisplayDriver<FoodPart>
{
    public override IDisplayResult Display(FoodPart part, BuildPartDisplayContext context) =>
        Initialize<FoodPartViewModel>(GetDisplayShapeType(context), viewModel => Populate(viewModel, part))
            .Location("Detail", "Meta:1")
            .Location("Summary", "Meta:1");

    public override IDisplayResult Edit(FoodPart part, BuildPartEditorContext context) =>
        Initialize<FoodPartViewModel>(GetEditorShapeType(context), viewModel => Populate(viewModel, part));

    public override async Task<IDisplayResult> UpdateAsync(FoodPart part, UpdatePartEditorContext context)
    {
        var viewModel = new FoodPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        if (!FoodRules.IsValidSpiciness(viewModel.Spiciness))
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(viewModel.Spiciness)}",
                stringLocalizer["The spiciness must be between 0 and {0}.", FoodRules.MaxSpiciness]);
        }

        part.Spiciness = viewModel.Spiciness;

        return Edit(part, context);
    }

    private static void Populate(FoodPartViewModel viewModel, FoodPart part)
    {
        viewModel.Spiciness = part.Spiciness;
        viewModel.FoodPart = part;
    }
}
