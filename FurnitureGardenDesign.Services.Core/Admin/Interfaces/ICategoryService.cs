using FurnitureGardenDesign.Web.ViewModels.Category;
namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesAsync();

        Task AddCategoryAsync(CategoryViewModelCreate model);

        Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id);

        Task EditCategoryAsync(Guid id, CategoryViewModelEdit model);

        Task ToggleCategoryAsync(Guid id);

        Task<IEnumerable<CategoryViewModelList>> GetAllCategoriesForAdminAsync();
    }

}