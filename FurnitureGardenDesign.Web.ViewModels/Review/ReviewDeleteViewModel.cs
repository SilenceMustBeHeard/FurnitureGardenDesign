using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Review
{
    public class ReviewDeleteViewModel
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
        public string UserName { get; set; } = null!;
        public string CatalogName { get; set; } = null!;
    }
}
