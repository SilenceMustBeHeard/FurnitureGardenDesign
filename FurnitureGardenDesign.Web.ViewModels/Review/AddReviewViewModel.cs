using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Review
{
    public class AddReviewViewModel
    {
    

        public Guid CatalogDesignId { get; set; }

      


        [Range(0, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

    }
}
