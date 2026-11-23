using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Handlers;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Handlers;

public sealed class DrinkPartHandler(IStringLocalizer<DrinkPartHandler> stringLocalizer)
    : ContentPartHandler<DrinkPart>
{
    public override Task ValidatingAsync(ValidateContentContext context, DrinkPart instance)
    {
        if (!DrinkRules.IsValidVolume(instance.VolumeMl))
        {
            context.Fail(stringLocalizer["The volume must be greater than zero."], nameof(DrinkPart.VolumeMl));
        }

        return Task.CompletedTask;
    }
}
