namespace RestaurantMenu.Module.Services;

public sealed record RestaurantInfo(string Name, string Address, string OpeningHours, string Currency);

public interface IRestaurantContext
{
    Task<RestaurantInfo> GetCurrentAsync();
}
