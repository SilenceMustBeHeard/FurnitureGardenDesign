using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.DesignVariants
{
    public class DesignVariantViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        // Image URL for the design variant

        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url]
        public string Image2DUrl { get; set; } = null!;

        [Required(ErrorMessage = "3D model URL is required.")]
        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        [Url]
        public string? Model3DUrl { get; set; } = null!;



        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Notes can only contain letters, numbers, spaces, and hyphens.")]
        public string? Notes { get; set; }



        public bool IsApproved { get; set; }


        public Order Order { get; set; } = null!;


    }
}
