using FurnitureGardenDesign.Web.ViewModels.Category;

namespace FurnitureGardenDesign.Services.Core.Manager.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesAsync();
    }
}