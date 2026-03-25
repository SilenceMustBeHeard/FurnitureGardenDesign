using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations.Catalog
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

        // For admin panel
        public async Task<IEnumerable<CategoryViewModelList>> GetAllCategoriesForAdminAsync()
        {
            var categories = await _categoryRepository.GetAllForAdminAsync();

            return categories.Select(c => new CategoryViewModelList
            {
                Id = c.Id,
                Name = c.Name,
                IsDeleted = c.IsDeleted
            });
        }

        // Add new category 

        public async Task AddCategoryAsync(CategoryViewModelCreate model)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
            };

            await _categoryRepository.AddAsync(category);
        }


        // Get category for edit (including deleted ones)

        public async Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
            if (category == null)
                return null;

            return new CategoryViewModelEdit
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsDeleted = category.IsDeleted
            };
        }


        // Edit category (including toggling IsDeleted)
        public async Task EditCategoryAsync(Guid id, CategoryViewModelEdit model)
        {
            var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
            if (category == null) return;

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsDeleted = model.IsDeleted;

            await _categoryRepository.UpdateAsync(category);
        }


        // Toggle category status (soft delete/restore)
        public async Task ToggleCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
            if (category == null) return;

            await _categoryRepository.ToggleCategoryStatusAsync(category);
        }
    }
}
