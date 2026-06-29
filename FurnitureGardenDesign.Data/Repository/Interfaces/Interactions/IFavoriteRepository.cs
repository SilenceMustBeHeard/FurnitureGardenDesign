using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Interactions
{
    public interface IFavoriteRepository
     : IRepository<Favorite, Guid>, IRepositoryAsync<Favorite, Guid>
    {
        Task<Favorite?> GetByCompositeKeyAsync(string userId, Guid designId);

        Task<bool> ExistsAsync(string userId, Guid designId);
    }
}