using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;

namespace FurnitureGardenDesign.Services.Core.Manager.Interfaces
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // For public display
        public async Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllActiveAsync();

            return categories.Select(c => new CategoryViewModelList
            {
                Id = c.Id,
                Name = c.Name,
                IsDeleted = c.IsDeleted
            });
        }

        
    }
}
