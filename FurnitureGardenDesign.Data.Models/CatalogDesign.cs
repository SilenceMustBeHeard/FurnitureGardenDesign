using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureGardenDesign.Data.Models
{
    public class CatalogDesign : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        // Title of the design
        public string Title { get; set; } = null!;


        // Foreign key to Category
        public Guid CategoryId { get; set; }
        public virtual  Category Category { get; set; } = null!;


        // Description of the design
        [Required]
        [MinLength(5)]
        public string Description { get; set; } = null!;

        // URLs for 2D image 
        [Url]
        public string Image2DUrl { get; set; } = null!;

        // URL for 3D model (optional)
        [Url]
        public string? Model3DUrl { get; set; }

        // Price of the design
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        // flag

        public bool IsActive { get; set; } = true;

        // flag to indicate if the design has a 3D model
        public Model3DStatus Model3DStatus { get; set; } = Model3DStatus.None;  



        // materials used
        public virtual ICollection<Material> Materials { get; set; }
         = new HashSet<Material>();

        // reviews given
        public  virtual ICollection<Review> Reviews { get; set; }
            = new HashSet<Review>();
  

        public virtual ICollection<Favorite> Favorites { get; set; }
            = new HashSet<Favorite>();
    }
}
