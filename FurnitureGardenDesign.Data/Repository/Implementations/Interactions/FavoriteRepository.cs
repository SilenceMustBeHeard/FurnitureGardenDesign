using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions
{
    public class FavoriteRepository :
          BaseRepository<Favorite, Guid>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Favorite?> GetByCompositeKeyAsync(string userId, Guid designId)

            => await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                   f.CatalogDesignId == designId);

        public async Task<bool> ExistsAsync(string userId, Guid designId)
            => await _dbSet
                .AnyAsync(f =>
                    f.UserId == userId &&
                    f.CatalogDesignId == designId &&
                    !f.IsDeleted);
    }
}