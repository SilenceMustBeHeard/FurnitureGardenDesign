using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Material
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        // Name of the material (e.g., "Wood", "Metal", "Plastic")
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Name can only contain letters, numbers, spaces, and hyphens.")]
        public string Name { get; set; } = null!;

        //flag

        public bool IsOutdoorSuitable { get; set; }
        // Optional description of the material
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Description can only contain letters, numbers, spaces, and hyphens.")]
        public string? Description { get; set; }

        // Navigation property for the catalog designs that use this material
        public virtual ICollection<CatalogDesign> CatalogDesigns { get; set; }
            = new HashSet<CatalogDesign>();
    }
}
