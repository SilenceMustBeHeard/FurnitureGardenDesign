using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }
        public async Task AddToFavoritesAsync(string userId, Guid catalogDesignId)
        {
            bool exists = await _favoriteRepository
                .FirstOrDefaultAsync(f => f.UserId == userId
                && f.CatalogDesignId == catalogDesignId) != null;



            if (exists)
            {
                return;
            }

            await _favoriteRepository.AddAsync(new Favorite
            {
                UserId = userId,
                CatalogDesignId = catalogDesignId
            });

      
        }

        public IEnumerable<Favorite> GetFavoritesByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromFavoritesAsync(string userId, Guid catalogDesignId)
        {
            throw new NotImplementedException();
        }
    }
}
