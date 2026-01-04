using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels
{
   
    public class OrderFormViewModel
    {
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [Display(Name = "Furniture Type")]
        public string FurnitureType { get; set; } = null!; // chair, table, e.t.c.

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [Display(Name = "Dimensions")]
        public string Dimensions { get; set; } = null!; // Example: 200x80x75 cm

        [Required]
        [MinLength(3)]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!; // short description

        [Url]
        [Display(Name = "Reference Image URL")]
        public string? ReferenceImageUrl { get; set; } // optional reference image

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; } // choice of category
    }
}
