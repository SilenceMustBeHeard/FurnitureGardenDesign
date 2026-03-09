using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureGardenDesign.Web.ViewModels.DesignVariants
{
    public class DesignVariantViewModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url]
        public string Image2DUrl { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        [Url]
        public string? Model3DUrl { get; set; }


     

        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$",
            ErrorMessage = "Notes can only contain letters, numbers, spaces, and hyphens.")]
        public string? Notes { get; set; }

        public bool IsApproved { get; set; }

        //  Order Preview Info (UI only)
        public string? OrderDescription { get; set; }

        public string? OrderDimensions { get; set; }

        public string? ReferenceImageUrl { get; set; }
    }
}
