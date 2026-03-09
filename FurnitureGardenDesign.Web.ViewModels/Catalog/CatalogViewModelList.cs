using FurnitureGardenDesign.Data.Common.Enums;
using System;

namespace FurnitureGardenDesign.Web.ViewModels.Catalog
{
    public class CatalogViewModelList
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!; 
        public decimal Price { get; set; } 
        public Model3DStatus Model3DStatus { get; set; } 
        public bool IsDeleted { get; set; }
    }
}