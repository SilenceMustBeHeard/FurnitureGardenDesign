using FurnitureGardenDesign.Data.Models.Interactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models.Catalog
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        // Name of the category, e.g., "Living Room", "Bedroom", etc.
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        // Optional description of the category
        public string? Description { get; set; }


        //flag
        public bool IsDeleted { get; set; } = false;


        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        = new HashSet<Order>();

        public virtual ICollection<CatalogDesign> CatalogDesigns { get; set; }
            = new HashSet<CatalogDesign>();
    }
}
