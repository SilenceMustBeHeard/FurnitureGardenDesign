using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Data.Models.Messages
{
    public class SystemInboxMessage : BaseDeletableEntity
    {
        public Guid Id { get; set; }
    
        public string ReceiverId { get; set; } = null!;
        public string? SenderId { get; set; } 
        public InboxMessageType Type { get; set; }
        public bool IsRead { get; set; }

        
        [Required]
        [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
        public string Description { get; set; } = null!;

        // Navigation
        public virtual AppUser Receiver { get; set; } = null!;
        public virtual AppUser? Sender { get; set; }
    }


}
