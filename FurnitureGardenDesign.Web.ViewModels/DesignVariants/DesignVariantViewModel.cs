using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.DesignVariants
{
    public class DesignVariantViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string? Notes { get; set; }

        public bool IsApproved { get; set; }


    }
}
