using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
    public class InboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        // Optional foreign key to a design variant, in case the message is related to a specific design update or feedback
        public Guid? DesignVariantId { get; set; }
        public DesignVariant? DesignVariant { get; set; }

        // Foreign key to the receiver (customer) of the message

        public string ReceiverId { get; set; } = null!;
        public AppUser Receiver { get; set; } = null!;



        // Type of the message 
        public InboxMessageType Type { get; set; }


        // Indicates whether the message has been read by the receiver
        public bool IsRead { get; set; }

        // Timestamp for when the message was created
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }


}
