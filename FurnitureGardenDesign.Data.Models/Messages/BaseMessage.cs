using FurnitureGardenDesign.Data.Common.Enums;

namespace FurnitureGardenDesign.Data.Models.Messages
{
    public class BaseMessage : BaseDeletableEntity
    {
        public Guid Id { get; set; }

        public string ReceiverId { get; set; } = null!;
        public string? SenderId { get; set; }
        public InboxMessageType Type { get; set; }
        public bool IsRead { get; set; }

        // Navigation

        public virtual AppUser Receiver { get; set; } = null!;
        public virtual AppUser? Sender { get; set; }
    }
}