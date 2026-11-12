sln := "RestaurantMenu.sln"
web := "src/RestaurantMenu.Web"

default:
    @just --list

run:
    dotnet run --project {{web}} --launch-profile RestaurantMenu.Web

watch:
    dotnet watch --project {{web}} --launch-profile RestaurantMenu.Web

build:
    dotnet build {{sln}}

fmt:
    dotnet format {{sln}}

check:
    dotnet format {{sln}} --verify-no-changes
    dotnet build {{sln}} --no-restore -p:TreatWarningsAsErrors=true -p:WarningsNotAsErrors=NU1902%3BNU1903
    dotnet test {{sln}} --no-build

reset:
    rm -rf {{web}}/App_Data
