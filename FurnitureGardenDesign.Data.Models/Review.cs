using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Review : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CatalogDesignId { get; set; }

        public string UserId { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;


        [Range(0,5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        

        // Navigation
        public CatalogDesign CatalogDesign { get; set; } = null!;
    }
}
