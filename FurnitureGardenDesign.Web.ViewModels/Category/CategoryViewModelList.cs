using System;
using System.Collections.Generic;
using System.Text;
using System;

namespace FurnitureGardenDesign.Web.ViewModels.Category
{
    public class CategoryViewModelList
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}

