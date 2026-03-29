using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }

  
        // Navigation
        public virtual ICollection<Order> Orders { get; set; }
            = new HashSet<Order>();

        public virtual ICollection<Favorite> Favorites { get; set; } 
            = new HashSet<Favorite>();

        // Design messages (InboxMessage)
        public ICollection<InboxMessage> ReceivedDesignMessages { get; set; } 
            = new HashSet<InboxMessage>();

        public ICollection<InboxMessage> SentDesignMessages { get; set; } 
            = new HashSet<InboxMessage>();


        // System messages (SystemInboxMessage)
        public ICollection<SystemInboxMessage> ReceivedSystemMessages { get; set; }
            = new HashSet<SystemInboxMessage>();

        public ICollection<SystemInboxMessage> SentSystemMessages { get; set; }
            = new HashSet<SystemInboxMessage>();


        // Contact messages (ContactMessage)
        public ICollection<ContactMessage> ReceivedContactMessages { get; set; }
            = new HashSet<ContactMessage>();

        public ICollection<ContactMessage> SentContactMessages { get; set; } 
            = new HashSet<ContactMessage>();
    }
}