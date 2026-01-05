using FurnitureGardenDesign.Web.ViewModels.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesAsync();

    Task AddCategoryAsync(CategoryViewModelCreate model);

    Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id);

    Task EditCategoryAsync(Guid id, CategoryViewModelEdit model);

    Task SoftDeleteCategoryAsync(Guid id);
}
