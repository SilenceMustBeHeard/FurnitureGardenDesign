using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        //flag
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Order> Orders { get; set; }
        = new HashSet<Order>();

        public virtual ICollection<CatalogDesign> CatalogDesigns { get; set; }
            = new HashSet<CatalogDesign>();
    }
}
