using FurnitureGardenDesign.Data.Models.Interactions;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models.Catalog
{
    public class DesignVariant : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key to Order
        public Guid OrderId { get; set; }

        // Image URL for the design variant
 
        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url]
        public string Image2DUrl { get; set; } = null!;

      
        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        [Url]
        public string? Model3DUrl { get; set; } 


        // Optional notes about the design variant

        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Notes can only contain letters, numbers, spaces, and hyphens.")]
        public string? Notes { get; set; }


        // Indicates whether this design variant has been approved by the customer
        public bool IsApproved { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
