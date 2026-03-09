using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
    public class InboxMessage : BaseDeletableEntity
    {
        public Guid Id { get; set; }
        public Guid DesignVariantId { get; set; }
        public string ReceiverId { get; set; } = null!;
        public string? SenderId { get; set; } 
        public InboxMessageType Type { get; set; }
        public bool IsRead { get; set; }

        public string? Notes { get; set; }

        // Navigation 
        public virtual DesignVariant DesignVariant { get; set; } = null!;
        public virtual AppUser Receiver { get; set; } = null!;
        public virtual AppUser? Sender { get; set; }
    }


}
