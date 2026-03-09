
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


        public ICollection<InboxMessage> InboxMessages { get; set; } = new HashSet<InboxMessage>();

        
        public ICollection<InboxMessage> SentMessages { get; set; } = new HashSet<InboxMessage>();
    }
}
