using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Data.Models
{
    public class CatalogDesign
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [MinLength(3)]

        public string Title { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;


        [Required]

        [MinLength(5)]
        public string Description { get; set; } = null!;

        [Url]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Range(0, 99999.99)]
        public decimal Price { get; set; }

        // flag

        public bool IsActive { get; set; } = true;

 
        public virtual ICollection<Material> Materials { get; set; }
         = new HashSet<Material>();

        public  ICollection<Review> Reviews { get; set; }
            = new HashSet<Review>();
  
        public  ICollection<Favorite> Favorites { get; set; }
            = new HashSet<Favorite>();
    }
}
