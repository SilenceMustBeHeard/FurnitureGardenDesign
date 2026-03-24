using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations.Catalog
{
    public class CategoryServiceClient : ICategoryServiceClient
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryServiceClient(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        // retrieves all active categories for the client
        // and maps them to a list of CategoryViewModelList objects for dropdown display
        public async Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForClientAsync()
        {
            var categories = await _categoryRepository.GetAllActiveAsync();

            return categories.Select(c => new CategoryViewModelList
            {
                Id = c.Id,
                Name = c.Name
            });
        }

    }

}
