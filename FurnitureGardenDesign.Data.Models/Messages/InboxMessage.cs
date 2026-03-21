using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models.Catalog;
using System;

namespace FurnitureGardenDesign.Data.Models.Messages
{
    public class InboxMessage : BaseMessage
    {
        public Guid DesignVariantId { get; set; }
        public string? Notes { get; set; }

        // Navigation 
        public virtual DesignVariant DesignVariant { get; set; } = null!;
    }
}