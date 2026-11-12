using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Handlers;
using RestaurantMenu.Module.Models;
using RestaurantMenu.Module.Services;

namespace RestaurantMenu.Module.Handlers;

public sealed class MenuItemPartHandler : ContentPartHandler<MenuItemPart>
{
    internal readonly IStringLocalizer S;

    public MenuItemPartHandler(IStringLocalizer<MenuItemPartHandler> stringLocalizer) =>
        S = stringLocalizer;

    public override Task ValidatingAsync(ValidateContentContext context, MenuItemPart instance)
    {
        if (!PriceRules.IsValid(instance.Price))
        {
            context.Fail(
                S["The price must be greater than zero and have at most {0} decimal places.", PriceRules.MaxDecimalPlaces],
                nameof(MenuItemPart.Price));
        }

        return Task.CompletedTask;
    }
}
