using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;
using RestaurantMenu.Module.ViewModels;

namespace RestaurantMenu.Module.Drivers;

public sealed class DrinkPartDisplayDriver(IStringLocalizer<DrinkPartDisplayDriver> stringLocalizer)
    : ContentPartDisplayDriver<DrinkPart>
{
    public override IDisplayResult Display(DrinkPart part, BuildPartDisplayContext context) =>
        Initialize<DrinkPartViewModel>(GetDisplayShapeType(context), viewModel => Populate(viewModel, part))
            .Location("Detail", "Meta:1")
            .Location("Summary", "Meta:1");

    public override IDisplayResult Edit(DrinkPart part, BuildPartEditorContext context) =>
        Initialize<DrinkPartViewModel>(GetEditorShapeType(context), viewModel => Populate(viewModel, part));

    public override async Task<IDisplayResult> UpdateAsync(DrinkPart part, UpdatePartEditorContext context)
    {
        var viewModel = new DrinkPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        if (!DrinkRules.IsValidVolume(viewModel.VolumeMl))
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(viewModel.VolumeMl)}",
                stringLocalizer["The volume must be greater than zero."]);
        }

        part.VolumeMl = viewModel.VolumeMl;
        part.IsAlcoholic = viewModel.IsAlcoholic;

        return Edit(part, context);
    }

    private static void Populate(DrinkPartViewModel viewModel, DrinkPart part)
    {
        viewModel.VolumeMl = part.VolumeMl;
        viewModel.IsAlcoholic = part.IsAlcoholic;
        viewModel.DrinkPart = part;
    }
}
