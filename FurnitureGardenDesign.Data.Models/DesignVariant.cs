using System;

namespace FurnitureGardenDesign.Data.Models
{
    public class DesignVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key to Order
        public Guid OrderId { get; set; }

        // Image URL for the design variant
        public string ImageUrl { get; set; } = null!;


        // Optional notes about the design variant
        public string? Notes { get; set; }


        // Indicates whether this design variant has been approved by the customer
        public bool IsApproved { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
