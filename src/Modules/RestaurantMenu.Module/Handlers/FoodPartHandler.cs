using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Handlers;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Handlers;

public sealed class FoodPartHandler(IStringLocalizer<FoodPartHandler> stringLocalizer)
    : ContentPartHandler<FoodPart>
{
    public override Task ValidatingAsync(ValidateContentContext context, FoodPart instance)
    {
        if (!FoodRules.IsValidSpiciness(instance.Spiciness))
        {
            context.Fail(
                stringLocalizer["The spiciness must be between 0 and {0}.", FoodRules.MaxSpiciness],
                nameof(FoodPart.Spiciness));
        }

        return Task.CompletedTask;
    }
}
