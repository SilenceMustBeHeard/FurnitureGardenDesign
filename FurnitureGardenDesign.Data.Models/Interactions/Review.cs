using FurnitureGardenDesign.Data.Models.Catalog;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models.Interactions
{
    public class Review : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key to the catalog design being reviewed
        public Guid CatalogDesignId { get; set; }

        // Foreign key to the user who wrote the review
        public string UserId { get; set; } = null!;

        public virtual AppUser User { get; set; } = null!;

        // Rating from 0 to 5
        [Range(0, 5)]
        public int Rating { get; set; }

        // Optional comment
        public string? Comment { get; set; }

        // Navigation
        public CatalogDesign CatalogDesign { get; set; } = null!;
    }
}