
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FurnitureGardenDesign.Data.Models
{
    public class AppUser : IdentityUser
    {

        public string? FullName { get; set; }    // Personalization Purposes
        public string? Address { get; set; }     // by choice

        // Navigation
        public virtual ICollection<Order> Orders { get; set; }
            = new HashSet<Order>();

        public virtual ICollection<Favorite> Favorites { get; set; } 
            = new HashSet<Favorite>();
    }
}
