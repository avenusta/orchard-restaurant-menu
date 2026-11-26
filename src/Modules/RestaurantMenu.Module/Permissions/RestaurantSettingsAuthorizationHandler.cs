using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Security;
using OrchardCore.Settings;
using RestaurantMenu.Module.Drivers;

namespace RestaurantMenu.Module.Permissions;

public sealed class RestaurantSettingsAuthorizationHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.HasSucceeded
            || requirement.Permission.Name != SettingsPermissions.ManageGroupSettings.Name
            || context.Resource?.ToString() != RestaurantSettingsDisplayDriver.GroupId)
        {
            return;
        }

        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        if (await authorizationService.AuthorizeAsync(context.User, MenuPermissions.ManageMenu))
        {
            context.Succeed(requirement);
        }
    }
}
