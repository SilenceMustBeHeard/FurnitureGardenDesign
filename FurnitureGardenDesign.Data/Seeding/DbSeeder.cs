using FurnitureGardenDesign.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FurnitureGardenDesign.Data.Seeding
{
    public static class DbSeeder
    {
  //      public static async Task SeedCatalogAsync(ApplicationDbContext context)
  //      {
  //          if (await context.CatalogDesigns.AnyAsync())
  //              return;


  //          var jsonPath = Path.Combine(
  //    Directory.GetCurrentDirectory(),
  //    "wwwroot",
  //    "data",
  //    "catalog.json"
  //);

  //          if (!File.Exists(jsonPath))
  //          {
  //              throw new Exception($"catalog.json NOT FOUND at: {jsonPath}");
  //          }


  //          var json = await File.ReadAllTextAsync(jsonPath);
  //          var movies = JsonSerializer.Deserialize<List<CatalogDesign>>(json);

  //          if (movies != null && movies.Count > 0)
  //          {
  //              context.CatalogDesigns.AddRange(movies);
  //              await context.SaveChangesAsync();
  //          }
  //      }




    }
}