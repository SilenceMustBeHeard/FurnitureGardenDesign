using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Models.Messages;
using Microsoft.AspNetCore.Identity;

namespace FurnitureGardenDesign.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName =>
            $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }


        // Navigation - Orders
        public virtual ICollection<Order> Orders { get; set; }
            = new HashSet<Order>();


        // Navigation - Favorites
        public virtual ICollection<Favorite> Favorites { get; set; }
            = new HashSet<Favorite>();


        // Navigation - Design messages
        public virtual ICollection<InboxMessage> ReceivedDesignMessages { get; set; }
            = new HashSet<InboxMessage>();

        public virtual ICollection<InboxMessage> SentDesignMessages { get; set; }
            = new HashSet<InboxMessage>();


        // Navigation - System messages
        public virtual ICollection<SystemInboxMessage> ReceivedSystemMessages { get; set; }
            = new HashSet<SystemInboxMessage>();

        public virtual ICollection<SystemInboxMessage> SentSystemMessages { get; set; }
            = new HashSet<SystemInboxMessage>();


        // Navigation - Contact messages
        public virtual ICollection<ContactMessage> ReceivedContactMessages { get; set; }
            = new HashSet<ContactMessage>();

        public virtual ICollection<ContactMessage> SentContactMessages { get; set; }
            = new HashSet<ContactMessage>();
    }
}