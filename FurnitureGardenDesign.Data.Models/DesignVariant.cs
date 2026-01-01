using System;

namespace FurnitureGardenDesign.Data.Models
{
    public class DesignVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string? Notes { get; set; }  

        public bool IsApproved { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
