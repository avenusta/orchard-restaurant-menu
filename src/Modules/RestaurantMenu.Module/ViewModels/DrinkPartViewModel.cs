using Microsoft.AspNetCore.Mvc.ModelBinding;
using RestaurantMenu.Module.Models;

namespace RestaurantMenu.Module.ViewModels;

public class DrinkPartViewModel
{
    public int VolumeMl { get; set; }

    public bool IsAlcoholic { get; set; }

    [BindNever]
    public DrinkPart? DrinkPart { get; set; }
}
