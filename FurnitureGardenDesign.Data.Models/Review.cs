using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CatalogDesignId { get; set; }

        public string UserId { get; set; } = null!;

        [Range(0,5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation
        public CatalogDesign CatalogDesign { get; set; } = null!;
    }
}
