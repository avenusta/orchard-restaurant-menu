using OrchardCore.ContentManagement;
using RestaurantMenu.Module.Models;
using YesSql.Indexes;

namespace RestaurantMenu.Module.Indexes;

public sealed class MenuItemPartIndex : MapIndex
{
    public required string ContentItemId { get; set; }

    public required string ContentType { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public int SortOrder { get; set; }
}

public sealed class MenuItemPartIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context) =>
        context.For<MenuItemPartIndex>()
            .When(contentItem => contentItem.Has<MenuItemPart>())
            .Map(contentItem =>
            {
                var part = contentItem.Get<MenuItemPart>(nameof(MenuItemPart));

                return new MenuItemPartIndex
                {
                    ContentItemId = contentItem.ContentItemId,
                    ContentType = contentItem.ContentType,
                    Price = part.Price,
                    IsAvailable = part.IsAvailable,
                    SortOrder = part.SortOrder,
                };
            });
}
