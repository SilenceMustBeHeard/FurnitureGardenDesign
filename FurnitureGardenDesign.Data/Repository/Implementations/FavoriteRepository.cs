using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class FavoriteRepository:
          BaseRepository<Favorite, Guid>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Favorite?> GetByCompositeKeyAsync(string userId, Guid designId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                    f.CatalogDesignId == designId &&
                    !f.IsDeleted);
        }



        public async Task<bool> ExistsAsync(string userId, Guid designId)
        {
            return await _context.Favorites
                .AnyAsync(f =>
                    f.UserId == userId &&
                    f.CatalogDesignId == designId &&
                    !f.IsDeleted);
        }


    }
}
