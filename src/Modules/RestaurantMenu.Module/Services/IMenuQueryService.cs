using OrchardCore.ContentManagement;

namespace RestaurantMenu.Module.Services;

public interface IMenuQueryService
{
    Task<IReadOnlyList<MenuSection<ContentItem>>> GetMenuAsync();
}
