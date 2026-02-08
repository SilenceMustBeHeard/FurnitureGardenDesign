
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
        public virtual AppUser User { get; set; } = null!;

        // Order details

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        // chair, table, pergola, etc.
        public string FurnitureType { get; set; } = null!; 

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        // free text (e.g. 200x80x75 cm), 
        public string Dimensions { get; set; } = null!; 



        [Required]
        [MinLength(3)]
        // short text describing how it should look like
        public string Description { get; set; } = null!; 

      
        [Url]
        // optional image URL for reference
        public string? ReferenceImageUrl { get; set; }

        // Status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        //navigation properties for dropdown categories
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;



        // Navigation
        public virtual ICollection<DesignVariant> DesignVariants { get; set; }
            = new HashSet<DesignVariant>();
    }
}
