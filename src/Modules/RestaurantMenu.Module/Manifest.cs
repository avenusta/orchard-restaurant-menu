using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "RestaurantMenu.Module",
    Author = "avenusta",
    Website = "https://orchardcore.net",
    Version = "0.1.0",
    Description = "Menu items, categories and related stuff for a restaurant site",
    Category = "Content Management",
    Dependencies =
    [
        "OrchardCore.Contents",
        "OrchardCore.Title",
        "OrchardCore.Autoroute",
    ]
)]
