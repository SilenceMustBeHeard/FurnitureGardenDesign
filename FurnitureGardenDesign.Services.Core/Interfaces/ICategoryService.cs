using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(Category model);
        Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
    }
}
