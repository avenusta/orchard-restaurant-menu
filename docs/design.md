# Restaurant Menu: design

Orchard Core 3.0.1 (.NET 10) site for a single restaurant, built for the Óbuda University K-MOOC Orchard Core course. Requirements and instructor feedback are in `raw/02-requirements-and-milestones.md` and `raw/04-my-topic.md`.

## Architecture

- One Orchard app, server-rendered by a custom Razor theme. No React or SPA.
- One restaurant now. More restaurants are added later as **tenants** set up from the same recipe, with no data-model changes.
- Visitors browse only: no reservations or ordering.

```
src/RestaurantMenu.Web                 thin host (occms template), no features
src/Modules/RestaurantMenu.Module      all functionality, one module, one feature
src/Themes/RestaurantMenu.Theme        all visuals, Razor, built from scratch
test/RestaurantMenu.Module.Tests       xUnit, pure logic only
```

Principles:
- Composition over inheritance. Parts inherit from `ContentPart` only because Orchard requires it; there are no custom base classes. Types get their behaviour from the parts they combine.
- Services are written against interfaces.
- Every content type, part, field and taxonomy is defined in **migrations**, never only through the admin UI.

## Content model

### MenuItemPart (shared by Food and Drink)

| Member | Type | Indexed |
|---|---|---|
| Price | `decimal` | ✔ |
| IsAvailable | `bool` | ✔ |
| DietaryTags | `[Flags] enum DietaryTags { None, Vegetarian, Vegan, GlutenFree }` | ✔ |
| SortOrder | `int` | ✔ |
| Description | TextField (multi-line) | |
| Photo | MediaField | |
| Category | TaxonomyField → *Menu Categories* (required, single) | |

### FoodPart

| Member | Type |
|---|---|
| Spiciness | `int` from 0 to 3, shown as chili icons |

### DrinkPart

| Member | Type |
|---|---|
| VolumeMl | `int`, must be greater than zero |
| IsAlcoholic | `bool`, shown as an "18+" badge |

### Content types

| Type | Parts |
|---|---|
| Food | TitlePart, AutoroutePart (`/menu/{slug}`), MenuItemPart, FoodPart |
| Drink | TitlePart, AutoroutePart (`/menu/{slug}`), MenuItemPart, DrinkPart |
| LandingPage | TitlePart, AutoroutePart, FlowPart (contains HeroWidget and TextWidget). The home page is created by the recipe and set as the home route. |
| HeroWidget | Widget stereotype; Heading, Lead (TextFields) and Link (LinkField) |
| TextWidget | Widget stereotype; HtmlBodyPart |

### Taxonomies (created in migrations)

- **Menu Categories:** Starters, Soups, Mains, Desserts, … The taxonomy is created in the migration; its terms (`MenuCategory` items) are seeded by the recipe. There are no category pages: `/menu` has a section per category.

### Index

`MenuItemPartIndex`: ContentItemId, ContentType, Price, IsAvailable, DietaryTags, SortOrder.

## Restaurant info

```csharp
public interface IRestaurantContext
{
    Task<RestaurantInfo> GetCurrentAsync();
}
```

- v1 implementation: `SiteSettingsRestaurantContext`, reading `RestaurantSettings` from site settings (Name, Address, OpeningHours as free text, Currency code, default HUF).
- Templates and services only use `IRestaurantContext`, so the storage can later move to a Restaurant content type without changing callers.
- Prices are formatted by a price ad-hoc shape using the currency from `IRestaurantContext`: `2 490 Ft` for HUF (no decimals), `12.50 €` for EUR.

## Module components

| Component | Responsibility |
|---|---|
| Migrations | Create parts, fields, types, both taxonomies, the index table, and Autoroute patterns |
| `MenuItemPartDisplayDriver`, `FoodPartDisplayDriver`, `DrinkPartDisplayDriver` | Display and edit using ViewModels; validate in `UpdateAsync` |
| `MenuItemPartHandler` | Validates the price (> 0, max 2 decimals); normalises DietaryTags (Vegan implies Vegetarian) |
| `MenuItemPartIndexProvider` | Maps content items that have a MenuItemPart into `MenuItemPartIndex` |
| `IMenuQueryService` | Applies `MenuFilter` (diet, available only) to the index; groups results by category |
| `MenuController` | `[Route("menu")]`, the public menu grouped by category, with filters in the query string |
| `RestaurantSettings` display driver | Settings section "Restaurant" |
| `Permissions` | `ManageMenu`, granted to Administrator and Editor by default |
| `RestaurantSettingsAuthorizationHandler` | Lets `ManageMenu` open the "restaurant" site settings group, which Orchard otherwise guards with its generic settings permission |
| `AdminMenu : INavigationProvider` | "Restaurant" menu → Settings, Menu overview |
| `Admin/MenuOverviewController` | Lists all items by category with price and availability, plus an availability toggle (POST). Authorizes against `ManageMenu`. |
| Recipe `restaurant-menu.recipe.json` | Setup recipe: enables features, sets the theme, configures the Lucene index and search, adds sample categories, foods and drinks, and the landing page |

All user-facing strings use `S[]`/`H[]`. The site is English only for now.

## Public URLs

| URL | Source |
|---|---|
| `/` | Landing page (FlowPart) |
| `/menu` | `MenuController.Index` |
| `/menu#{category}` | Jump to a category section |
| `/menu?diet=vegan&available=true` | Same page, filtered |
| `/menu/{slug}` | Autoroute → Food/Drink detail |
| `/search?q=` | Lucene search |
| `/admin/restaurant/menu` | Admin menu overview |

Unavailable items are listed but styled greyed out with a "Sold out" label. The `available=true` filter hides them.

## Theme

- `Layout.cshtml` written from scratch (replacing the template's `Layout.liquid`), with own CSS and JS as static resources.
- Shape templates: `Food.cshtml`, `Food.Summary.cshtml`, `Drink.cshtml`, `Drink.Summary.cshtml`, and part templates.
- `placement.json` to position and hide part and field shapes.
- Ad-hoc shapes for the filter bar and price display.
- Opening hours and address in the footer, via `IRestaurantContext`.

## Tests

`RestaurantMenu.Module.Tests` (xUnit) covers pure logic only: price, spiciness and volume validation, menu grouping, DietaryTags normalisation, `MenuFilter` parsing, and price formatting. It runs as part of `just check`.

## Out of scope (possible additions later)

React, reservations, ordering, specials widget, open-now layout filter, background tasks, caching, workflows, a second language.
