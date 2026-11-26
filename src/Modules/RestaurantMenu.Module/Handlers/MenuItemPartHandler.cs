using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Handlers;

public sealed class MenuItemPartHandler(IStringLocalizer<MenuItemPartHandler> stringLocalizer)
    : ContentPartHandler<MenuItemPart>
{
    public override Task CreatingAsync(CreateContentContext context, MenuItemPart instance) =>
        NormalizeDietaryTagsAsync(instance);

    public override Task UpdatingAsync(UpdateContentContext context, MenuItemPart instance) =>
        NormalizeDietaryTagsAsync(instance);

    public override Task ValidatingAsync(ValidateContentContext context, MenuItemPart instance)
    {
        if (!PriceRules.IsValid(instance.Price))
        {
            context.Fail(
                stringLocalizer["The price must be greater than zero and have at most {0} decimal places.", PriceRules.MaxDecimalPlaces],
                nameof(MenuItemPart.Price));
        }

        return Task.CompletedTask;
    }

    private static Task NormalizeDietaryTagsAsync(MenuItemPart part)
    {
        part.DietaryTags = DietaryRules.Normalize(part.DietaryTags);
        part.ContentItem.Apply(nameof(MenuItemPart), part);

        return Task.CompletedTask;
    }
}
