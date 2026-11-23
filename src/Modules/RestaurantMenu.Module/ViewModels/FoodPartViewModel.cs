using Microsoft.AspNetCore.Mvc.ModelBinding;
using RestaurantMenu.Module.Models;

namespace RestaurantMenu.Module.ViewModels;

public class FoodPartViewModel
{
    public int Spiciness { get; set; }

    [BindNever]
    public FoodPart? FoodPart { get; set; }
}
