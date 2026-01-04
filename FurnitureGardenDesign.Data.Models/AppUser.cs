
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FurnitureGardenDesign.Data.Models
{
    public class AppUser : IdentityUser
    {

        public string? FullName { get; set; }    // Personalization Purposes
        public string? Address { get; set; }     // by choice

        // Navigation
        public virtual ICollection<OrderFormViewModel> Orders { get; set; }
            = new HashSet<OrderFormViewModel>();

        public virtual ICollection<Favorite> Favorites { get; set; } 
            = new HashSet<Favorite>();
    }
}
