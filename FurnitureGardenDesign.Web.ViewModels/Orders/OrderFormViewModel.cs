using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels
{
    public class OrderFormViewModel
    {
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [Display(Name = "Furniture Type")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Furniture type can only contain letters, numbers, spaces, and hyphens.")]
        public string FurnitureType { get; set; } = null!; // chair, table, e.t.c.

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [Display(Name = "Dimensions")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Dimensions can only contain letters, numbers, spaces, and hyphens.")]
        public string Dimensions { get; set; } = null!; // Example: 200x80x75 cm

        [Required]
        [MinLength(3)]
        [Display(Name = "Description")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Description can only contain letters, numbers, spaces, and hyphens.")]
        public string Description { get; set; } = null!; // short description

        [Url]
        [Display(Name = "Reference Image URL")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string? ReferenceImageUrl { get; set; } // optional reference image

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; } // choice of category

        public IEnumerable<SelectListItem> Categories { get; set; }
            = new List<SelectListItem>();
    }
}