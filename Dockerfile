FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "FurnitureGardenDesign.Web/FurnitureGardenDesign.Web.csproj"

RUN dotnet publish "FurnitureGardenDesign.Web/FurnitureGardenDesign.Web.csproj" \
    -c Release \
    -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && apt-get install -y curl

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FurnitureGardenDesign.Web.dll"]