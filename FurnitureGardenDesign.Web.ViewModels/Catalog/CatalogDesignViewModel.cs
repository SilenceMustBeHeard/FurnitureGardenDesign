using FurnitureGardenDesign.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Catalog
{
    public class CatalogDesignViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image2DUrl { get; set; } = null!;
        public string? Model3DUrl { get; set; }
        public Model3DStatus Model3DStatus { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsFavorited { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

      
    }
}
