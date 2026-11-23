# Implementation plan

Built from `docs/design.md`. Each slice is a vertical, deployable increment. When a slice is done:

- `just check` passes (formatting, build, tests)
- `just reset` followed by setup with the RestaurantMenu recipe gives a working site
- the slice's demo steps work in the browser
- everything is committed and pushed

Order is chosen so the **passing requirements** are met as early as possible: slices 1–2 already give a custom part, a custom type with migration, a driver, a handler, an index, and a custom theme. Later slices add breadth.

---

## Slice 0: Baseline and tooling

- Fix the build so `just check` passes on the empty solution.
- Add `test/RestaurantMenu.Module.Tests` (xUnit), add it to the solution, and make `just check` run `dotnet test`.
- Theme: replace `Layout.liquid` with a minimal `Layout.cshtml` (header, main zone, footer) and an empty `site.css` as a static resource.
- Recipe `restaurant-menu.recipe.json` (setup recipe): enables the module, the theme, and the core features needed (Contents, Title, Autoroute, Taxonomies, Media, Flow), and sets the site theme.
- First commit, then create the public GitHub repo.

**Demo:** reset, set up with "Restaurant Menu", and the site renders with the custom layout.

## Slice 1: Food with MenuItemPart (core content)

- `Models/MenuItemPart` (Price, IsAvailable, SortOrder).
- `Migrations/MenuItemMigrations`: the MenuItemPart definition, the `Food` type (Title, Autoroute with the `/menu/{slug}` pattern, MenuItemPart), and the `MenuItemPartIndex` table.
- `Indexes/MenuItemPartIndex` with its provider.
- `Drivers/MenuItemPartDisplayDriver` with `ViewModels/MenuItemPartViewModel`, and edit and display shapes.
- `Handlers/MenuItemPartHandler`: price validation.
- Price-validation logic as a pure static class, with unit tests.
- Theme: `Food.cshtml` detail template and `MenuItemPart.cshtml`.

**Demo:** create a Food item in the admin, see validation reject a price of 0, and view the item at `/menu/{slug}`. The index row is visible in DB Browser for SQLite.

## Slice 2: Categories and the public /menu page

- Migration `UpdateFrom1Async`: the `MenuCategory` term type, the *Menu Categories* taxonomy (fixed content item ID), and Category (TaxonomyField, required, single), Description (multi-line TextField) and Photo (MediaField, single) on MenuItemPart.
- `Services/MenuGrouping`: pure grouping by category in taxonomy order, items sorted by SortOrder then title, with unit tests.
- `Services/IMenuQueryService` and `MenuQueryService`: published items from the index, grouped with `MenuGrouping`.
- `Controllers/MenuController` with `[Route("menu")]`: one page, a section per category, anchor links (`/menu#soups`).
- Theme: `Content-Food.Summary.cshtml`, a greyed-out "Sold out" line, `placement.json` for field positions, and a hardcoded "Menu" link in the layout header.
- Recipe: four sample categories and eight sample foods, no photos.

**Demo:** `/menu` lists the foods grouped by category; an unavailable item shows as "Sold out"; a food without a category can't be published.

✅ After this slice the minimum passing requirements are met.

## Slice 3: Drinks, FoodPart and DrinkPart

- `Models/FoodPart` (Spiciness, PortionGrams) and `Models/DrinkPart` (VolumeMl, IsAlcoholic, AbvPercent).
- Migration: both part definitions, the `Drink` type, and FoodPart added to `Food`.
- Drivers for both parts. DrinkPart validates that AbvPercent is set when the drink is alcoholic.
- ABV validation logic with unit tests.
- Theme: `Drink.cshtml`, `Drink.Summary.cshtml`, `FoodPart.cshtml` (spiciness icons) and `DrinkPart.cshtml`.
- Recipe: sample drinks, and food items updated with spiciness and portion.

**Demo:** foods and drinks are mixed on `/menu`; an alcoholic drink without ABV is rejected.

## Slice 4: Dietary tags, allergens, filters

- The `DietaryTags` flags enum on MenuItemPart and in the index (migration update). The handler normalises tags (Vegan implies Vegetarian).
- Migration: the *Allergens* taxonomy with the 14 EU allergens, and an Allergens TaxonomyField on MenuItemPart.
- `MenuFilter` (parses diet, maxPrice, free and available from the query string). `MenuQueryService` applies it to the index and the taxonomy index.
- Unit tests for filter parsing and tag normalisation.
- Theme: the filter bar as an ad-hoc shape, plus diet badges and allergen icons on summaries.

**Demo:** `/menu?diet=vegan&free=nuts&maxPrice=3000&available=true` filters correctly.

## Slice 5: Restaurant settings, IRestaurantContext, permissions, admin menu

- `Settings/RestaurantSettings` (Name, Address, OpeningHours, Currency) with its display driver, shown as the "Restaurant" settings section.
- `Services/IRestaurantContext` and `SiteSettingsRestaurantContext`.
- Price formatting using the currency and culture. A price ad-hoc shape replaces the raw decimal everywhere.
- `Permissions/MenuPermissions` with `ManageMenu` (granted to Administrator and Editor).
- `AdminMenu : INavigationProvider` adds Restaurant → Settings.
- Theme: the footer shows the name, address and opening hours.
- Unit tests for opening-hours formatting.
- Recipe: sample restaurant settings.

**Demo:** edit the restaurant settings and see the footer and currency change; a user without `ManageMenu` doesn't see the menu entry.

## Slice 6: Admin menu overview

- `Controllers/Admin/MenuOverviewController` at `/admin/restaurant/menu`: lists all items by category with price and availability. A POST toggles availability. Both actions authorize against `ManageMenu`.
- Admin views, and a "Menu overview" entry in the AdminMenu.
- Uses `IMenuQueryService`, so there's no duplicated query code.

**Demo:** toggle an item to sold out from the overview and see it greyed out on `/menu`; a forbidden user gets 403.

## Slice 7: Landing page and search

- Recipe: the landing Page (FlowPart: hero, intro, link to `/menu`) set as the homepage, and a Lucene index plus search settings covering Food and Drink.
- Theme: templates for the landing page and the search results.

**Demo:** the homepage looks finished, and `/search?q=goulash` finds the dish.

## Slice 8: Demo hardening

- XML doc comments on all public types, and removal of any leftover template or sample code.
- A README covering what it is, how to run it, the features list, and the Orchard features used.
- A full reset → setup → click-through pass with no errors in the log.
- A demo script: the order in which to show the code (module, then theme), then the user flow.

---

## Timing

The course page lists the project presentation for **30 Nov – 6 Dec** and the early demo for **16 – 22 Nov**. Today is 11 Nov, so if those dates apply to your semester, about 3 weeks remain. Check the dates on the course forum. Slices 0–2 are the priority, because they meet the passing requirements; slices 3–7 add the breadth the instructors asked for.
