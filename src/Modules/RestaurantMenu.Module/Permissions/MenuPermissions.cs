using OrchardCore;
using OrchardCore.Security.Permissions;

namespace RestaurantMenu.Module.Permissions;

public static class MenuPermissions
{
    public static readonly Permission ManageMenu = new("ManageMenu", "Manage the restaurant menu and settings");
}

public sealed class MenuPermissionProvider : IPermissionProvider
{
    private readonly IEnumerable<Permission> _allPermissions = [MenuPermissions.ManageMenu];

    public Task<IEnumerable<Permission>> GetPermissionsAsync() => Task.FromResult(_allPermissions);

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
    [
        new PermissionStereotype { Name = OrchardCoreConstants.Roles.Administrator, Permissions = _allPermissions },
        new PermissionStereotype { Name = OrchardCoreConstants.Roles.Editor, Permissions = _allPermissions },
    ];
}
