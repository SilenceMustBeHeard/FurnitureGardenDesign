using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class DesignVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key to Order
        public Guid OrderId { get; set; }

        // Image URL for the design variant
        [Url]
        public string ImageUrl { get; set; } = null!;


        // Optional notes about the design variant
        
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Notes can only contain letters, numbers, spaces, and hyphens.")]
        public string? Notes { get; set; }


        // Indicates whether this design variant has been approved by the customer
        public bool IsApproved { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
