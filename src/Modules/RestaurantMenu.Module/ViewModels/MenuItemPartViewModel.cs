using Microsoft.AspNetCore.Mvc.ModelBinding;
using RestaurantMenu.Module.Models;

namespace RestaurantMenu.Module.ViewModels;

public class MenuItemPartViewModel
{
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public int SortOrder { get; set; }

    public bool IsVegetarian { get; set; }

    public bool IsVegan { get; set; }

    public bool IsGlutenFree { get; set; }

    [BindNever]
    public MenuItemPart? MenuItemPart { get; set; }
}
