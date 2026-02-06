using FurnitureGardenDesign.Web.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface ICategoryServiceClient
    {
        Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForClientAsync();

    }
}
