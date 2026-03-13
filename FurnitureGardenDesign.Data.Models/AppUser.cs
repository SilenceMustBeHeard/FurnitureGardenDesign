
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FurnitureGardenDesign.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }

        // Navigation
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();

        // inbox messages navigation for design variants 
        public ICollection<InboxMessage> InboxMessages { get; set; } = new HashSet<InboxMessage>();
        public ICollection<InboxMessage> SentMessages { get; set; } = new HashSet<InboxMessage>();


        // inbox messages navigation for system messages
        public ICollection<SystemInboxMessage> SystemInboxMessages { get; set; } = new HashSet<SystemInboxMessage>();
        public ICollection<SystemInboxMessage> SentSystemInboxMessages { get; set; } = new HashSet<SystemInboxMessage>();


    }
}
