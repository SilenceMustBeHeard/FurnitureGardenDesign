using FurnitureGardenDesign.Data.Models.Catalog;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FurnitureGardenDesign.Data.Seeding
{
    public static class DbSeeder
    {
        private static bool _isSeeding = false;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // Seed Categories from JSON file
        public static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            await _semaphore.WaitAsync();
            try
            {
                // Double-check after acquiring lock
                if (await context.Categories.AnyAsync())
                {
                    Console.WriteLine("Categories already exist. Skipping.");
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
                        throw new Exception("Category Name is NULL or EMPTY");

                    if (string.IsNullOrWhiteSpace(category.Description))
                        throw new Exception($"Category Description is NULL for: {category.Name}");

                    if (category.Id == Guid.Empty)
                        throw new Exception($"Category Id is EMPTY for: {category.Name}");

                    // Set to false (active)
                    category.IsDeleted = false;
                }

                // Use AddRange with Try-Catch for duplicate handling
                foreach (var category in categories)
                {
                    if (!await context.Categories.AnyAsync(c => c.Id == category.Id))
                    {
                        await context.Categories.AddAsync(category);
                    }
                }

                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Seeded {categories.Count} categories.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Seed CatalogDesigns from JSON file
        public static async Task SeedCatalogAsync(ApplicationDbContext context)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (await context.CatalogDesigns.AnyAsync())
                {
                    Console.WriteLine("Catalog designs already exist. Skipping.");
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
                        throw new Exception("CatalogDesign Title is NULL or EMPTY");

                    if (string.IsNullOrWhiteSpace(design.Description))
                        throw new Exception($"CatalogDesign Description is NULL for: {design.Title}");

                    if (string.IsNullOrWhiteSpace(design.Image2DUrl))
                        throw new Exception($"CatalogDesign Image2DUrl is NULL for: {design.Title}");

                    if (design.CategoryId == Guid.Empty)
                        throw new Exception($"CatalogDesign CategoryId is EMPTY for: {design.Title}");

                  
                    var categoryExists = await context.Categories.AnyAsync(c => c.Id == design.CategoryId);

                    if (!categoryExists)
                        throw new Exception($"CategoryId {design.CategoryId} for {design.Title} does not exist.");

                    if (design.Price <= 0)
                        throw new Exception($"CatalogDesign Price is INVALID for: {design.Title}");

                    design.IsActive = true;
                }

                // Add each design individually to avoid bulk duplicate issues
                foreach (var design in designs)
                {
                    if (!await context.CatalogDesigns.AnyAsync(cd => cd.Id == design.Id))
                    {
                        await context.CatalogDesigns.AddAsync(design);
                    }
                }

                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Seeded {designs.Count} catalog designs.");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}