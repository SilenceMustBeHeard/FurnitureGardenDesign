using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models.Interactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureGardenDesign.Data.Models.Catalog
{
    public class CatalogDesign : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Title can only contain letters, numbers, spaces, and hyphens.")]
        public string Title { get; set; } = null!;

        // Foreign key to Category
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        // Description of the design
        [Required]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters long.")]
        public string Description { get; set; } = null!;

        // URLs for 2D image

        [Url]
        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string Image2DUrl { get; set; } = null!;

        // URL for 3D model (optional)
        [Url]
        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        public string? Model3DUrl { get; set; }

        // Price of the design
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // flag

        public bool IsActive { get; set; } = true;

        // flag to indicate if the design has a 3D model
        public Model3DStatus Model3DStatus { get; set; } = Model3DStatus.None;

        // materials used

        [MaxLength(700, ErrorMessage = "Materials cannot exceed 700 characters.")]
        public string? Materials { get; set; }

        // reviews given
        public virtual ICollection<Review> Reviews { get; set; }
            = new HashSet<Review>();

        public virtual ICollection<Favorite> Favorites { get; set; }
            = new HashSet<Favorite>();
    }
}