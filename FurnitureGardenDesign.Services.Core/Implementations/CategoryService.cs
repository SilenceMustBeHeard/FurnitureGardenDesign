using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllActiveAsync();

        return categories.Select(c => new CategoryViewModelList
        {
            Id = c.Id,
            Name = c.Name,
            IsActive = c.IsActive
        });
    }

    public async Task AddCategoryAsync(CategoryViewModelCreate model)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            IsActive = model.IsActive
        };

        await _categoryRepository.AddAsync(category);
    }

    public async Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryViewModelEdit
        {
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task EditCategoryAsync(Guid id, CategoryViewModelEdit model)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return;

        category.Name = model.Name;
        category.Description = model.Description;
        category.IsActive = model.IsActive;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task SoftDeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return;

        await _categoryRepository.DeleteAsync(category);
    }
}
