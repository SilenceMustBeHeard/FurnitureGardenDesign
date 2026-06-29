using FurnitureGardenDesign.Web.ViewModels.Category;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Catalog
{
    public interface ICategoryServiceClient
    {
        Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForClientAsync();
    }
}