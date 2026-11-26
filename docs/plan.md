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

- `Models/FoodPart` (Spiciness) and `Models/DrinkPart` (VolumeMl, IsAlcoholic).
- Migration `UpdateFrom2Async`: both part definitions, FoodPart added to `Food`, and the `Drink` type (Title, Autoroute `/menu/{slug}`, MenuItemPart, DrinkPart).
- Drivers for both parts. Spiciness must be 0–3 and volume must be greater than zero, each checked in its driver and a handler, with the rules as pure logic and unit tests.
- Theme: a shared menu-line partial used by `Content-Food.Summary` and `Content-Drink.Summary` (and the same for the detail templates); `FoodPart.cshtml` (chili icons) and `DrinkPart.cshtml` ("330 ml" and an "18+" badge).
- Recipe: drink categories (Soft drinks, Hot drinks, Wine & beer) in the same taxonomy, sample drinks, and spiciness on the sample foods.

**Demo:** foods and drinks are listed together on `/menu`; food lines show chili icons; drink lines show volume and "18+"; a drink with no volume is rejected.

## Slice 4: Dietary tags and the menu filter

- `DietaryTags` flags enum (Vegetarian, Vegan, GlutenFree) on MenuItemPart: editor checkboxes, a new `DietaryTags` column in `MenuItemPartIndex` (migration step), and the handler normalises tags (Vegan implies Vegetarian).
- `MenuFilter`: parses `diet` and `available` from the query string. `MenuQueryService` applies it to the index query.
- Unit tests for filter parsing and tag normalisation.
- Theme: a filter row on `/menu` as an ad-hoc shape (diet links plus "available only"), and diet badges on menu lines.
- Recipe: diet tags on the sample items.

**Demo:** `/menu?diet=vegan&available=true` shows only available vegan items; a Vegan item is also listed under Vegetarian.

## Slice 5: Restaurant settings, IRestaurantContext, permission, admin menu

- `Settings/RestaurantSettings` (Name, Address, OpeningHours as free text, Currency code) with its display driver, shown as the "Restaurant" settings section.
- `Services/IRestaurantContext` and `SiteSettingsRestaurantContext`.
- `PriceFormatter` (pure, unit-tested): `2 490 Ft` for HUF without decimals, `12.50 €` for EUR. A price ad-hoc shape replaces the raw decimal everywhere.
- `Permissions/MenuPermissions` with `ManageMenu` (granted to Administrator and Editor).
- `AdminMenu : INavigationProvider` adds Restaurant → Settings, visible with `ManageMenu`.
- Theme: the footer shows the name, address and opening hours.
- Recipe: sample restaurant settings.

**Demo:** edit the restaurant settings and see the footer and prices change; a user without `ManageMenu` doesn't see the Restaurant menu.

## Slice 6: Admin menu overview

- `Controllers/Admin/MenuOverviewController` at `/admin/restaurant/menu`: lists all items by category with price and availability. A POST toggles availability. Both actions authorize against `ManageMenu`.
- Admin views, and a "Menu overview" entry in the Restaurant admin menu.
- Uses `IMenuQueryService`, so there's no duplicated query code.

**Demo:** toggle an item to sold out from the overview and see it greyed out on `/menu`; a user without `ManageMenu` gets 403.

## Slice 7: Landing page and search

- Recipe: the landing Page (FlowPart: hero, intro, link to `/menu`) set as the homepage, so `/` no longer shows "page not found".
- Recipe: a Lucene index covering Food and Drink, and the Search feature configured to use it at `/search`.
- Theme: templates for the landing page and the search results (reusing the `MenuLine` shape).

**Demo:** the homepage shows the hero and links to the menu; `/search?q=goulash` finds the dish.

## Slice 8: Demo hardening

- XML doc comments on all public types, and removal of any leftover template or sample code.
- A README covering what it is, how to run it, the features list, and the Orchard features used.
- A full reset → setup → click-through pass with no errors in the log.
- A demo script: the order in which to show the code (module, then theme), then the user flow.

---

## Timing

The course page lists the project presentation for **30 Nov – 6 Dec** and the early demo for **16 – 22 Nov**. Today is 11 Nov, so if those dates apply to your semester, about 3 weeks remain. Check the dates on the course forum. Slices 0–2 are the priority, because they meet the passing requirements; slices 3–7 add the breadth the instructors asked for.
