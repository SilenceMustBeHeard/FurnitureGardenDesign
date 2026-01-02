
using Furniture_GardenDesign.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class Order : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Customer
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // Order details

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string FurnitureType { get; set; } = null!; // chair, table, pergola, etc.

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Dimensions { get; set; } = null!; // free text (e.g. 200x80x75 cm), 

        [Required]
     
        [MinLength(3)]
        public string Description { get; set; } = null!; // short text describing how it should look like

      
        [Url]
        public string? ReferenceImageUrl { get; set; }

        // Status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;


        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;


       



        // Navigation
        public virtual ICollection<DesignVariant> DesignVariants { get; set; }
            = new HashSet<DesignVariant>();
    }
}
