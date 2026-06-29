using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Catalog
{
    public interface ICategoryRepository :
         IRepository<Category, Guid>, IRepositoryAsync<Category, Guid>
    {
        Task<Category?> GetByIdIncludingDeletedAsync(Guid id);

        Task<IEnumerable<Category>> GetAllActiveAsync();

        Task<IEnumerable<Category>> GetAllForAdminAsync();

        Task ToggleCategoryStatusAsync(Category category);

        Category? GetByName(string name);
    }
}