using System;

namespace FurnitureGardenDesign.Data.Models
{
    public class Favorite
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;

        public Guid CatalogDesignId { get; set; }

   
        public CatalogDesign CatalogDesign { get; set; } = null!;
    }
}
