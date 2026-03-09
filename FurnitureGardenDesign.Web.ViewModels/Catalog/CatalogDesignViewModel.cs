using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Catalog
{
    public class CatalogDesignViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Title can only contain letters, numbers, spaces, and hyphens.")]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters long.")]
        public string Description { get; set; } = null!;


        [Url(ErrorMessage = "Image2DUrl must be a valid URL.")]
        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string Image2DUrl { get; set; } = null!;

        [Url(ErrorMessage = "Model3DUrl must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        public string? Model3DUrl { get; set; }

        public Model3DStatus Model3DStatus { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }


       
        public string CategoryName { get; set; } = null!;
        public bool IsFavorited { get; set; }


        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }


        public bool IsDeleted { get; set; } 
        public string StatusBadge => IsDeleted ? "Inactive" : "Active";
        public string StatusClass => IsDeleted ? "bg-secondary" : "bg-success";


    }
}
