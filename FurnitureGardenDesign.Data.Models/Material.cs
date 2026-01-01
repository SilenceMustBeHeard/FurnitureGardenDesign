using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Material
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        //flag

        public bool IsOutdoorSuitable { get; set; }

        public string? Description { get; set; }

      
        public virtual ICollection<CatalogDesign> CatalogDesigns { get; set; }
            = new HashSet<CatalogDesign>();
    }
}
