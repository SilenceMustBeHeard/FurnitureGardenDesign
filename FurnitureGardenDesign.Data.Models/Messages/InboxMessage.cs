using FurnitureGardenDesign.Data.Models.Catalog;

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