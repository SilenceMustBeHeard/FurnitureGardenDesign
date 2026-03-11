using FurnitureGardenDesign.Data.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Catalog
{
    public class CatalogViewModelEdit
    {

        public Guid Id { get; set; } 
        [Required]
        [MaxLength(100)]
        [MinLength(3)]

        public string Title { get; set; } = null!;



        
        [Required]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters long.")]
        public string Description { get; set; } = null!;


        [Url]
        [Required(ErrorMessage = "2D image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string Image2DUrl { get; set; } = null!;

 
        [Url]

        [MaxLength(500, ErrorMessage = "Model URL cannot exceed 500 characters.")]
        public string? Model3DUrl { get; set; }


        [MaxLength(700, ErrorMessage = "Materials cannot exceed 700 characters.")]
        public string? Materials { get; set; }

        public string Price { get; set; } = null!;


        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; } 


        public IEnumerable<SelectListItem> Categories { get; set; }
            = new List<SelectListItem>();
        public bool IsDeleted { get; set; } = true;


        public Model3DStatus Model3DStatus { get; set; } = Model3DStatus.None;

    }
}
