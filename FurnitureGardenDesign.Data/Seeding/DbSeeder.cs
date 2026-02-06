using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FurnitureGardenDesign.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedCategoriesAsync(context);
            await SeedCatalogAsync(context);
        }

        public static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var jsonPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data",
                "categories.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new Exception($"categories.json NOT FOUND at: {jsonPath}");
            }

            var json = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var categories = JsonSerializer.Deserialize<List<Category>>(json, options)
                ?? throw new Exception("categories.json is empty or invalid");

            foreach (var category in categories)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    throw new Exception("Category Name is NULL or EMPTY");
                }

                if (string.IsNullOrWhiteSpace(category.Description))
                {
                    throw new Exception($"Category Description is NULL for: {category.Name}");
                }

                if (category.Id == Guid.Empty)
                {
                    throw new Exception($"Category Id is EMPTY for: {category.Name}");
                }

                category.IsDeleted = true;
            }

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

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

         
                if (!await context.Categories.AnyAsync(c => c.Id == design.CategoryId))
                {
                    throw new Exception($"CategoryId {design.CategoryId} for {design.Title} does not exist.");
                }

                if (design.Price <= 0)
                {
                    throw new Exception($"CatalogDesign Price is INVALID for: {design.Title}");
                }

                design.IsActive = true;
            }

            await context.CatalogDesigns.AddRangeAsync(designs);
            await context.SaveChangesAsync();
        }
    }
}
