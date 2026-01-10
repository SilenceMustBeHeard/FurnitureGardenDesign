using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FurnitureGardenDesign.Data.Seeding
{
    public static class DbSeeder
    {


        

        public static async Task SeedCatalogAsync(ApplicationDbContext context)
        {
            if (await context.CatalogDesigns.AnyAsync())
            {
                return;
            }

            var jsonPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data",
                "catalog.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new Exception($"catalog.json NOT FOUND at: {jsonPath}");
            }

            var json = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var designs = JsonSerializer.Deserialize<List<CatalogDesign>>(json, options)
                ?? throw new Exception("catalog.json is empty or invalid");

            foreach (var design in designs)
            {
                if (string.IsNullOrWhiteSpace(design.Title))
                {
                    throw new Exception("CatalogDesign Title is NULL or EMPTY");
                }

                if (string.IsNullOrWhiteSpace(design.Description))
                {
                    throw new Exception($"CatalogDesign Description is NULL for: {design.Title}");
                }

                if (string.IsNullOrWhiteSpace(design.ImageUrl))
                {
                    throw new Exception($"CatalogDesign ImageUrl is NULL for: {design.Title}");
                }

                if (design.CategoryId == Guid.Empty)
                {
                    throw new Exception($"CatalogDesign CategoryId is EMPTY for: {design.Title}");
                }

                if (design.Price <= 0)
                {
                    throw new Exception($"CatalogDesign Price is INVALID for: {design.Title}");
                }


                design.IsActive = true;
            }

            context.CatalogDesigns.AddRange(designs);
            await context.SaveChangesAsync();
        }
    }
}