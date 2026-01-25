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
        public async Task ToggleFavoriteAsync(string userId, Guid designId)
        {
            var favorite = await _favoriteRepository.GetByCompositeKeyAsync(userId, designId);

            if (favorite == null)
            {
                favorite = new Favorite
                {
                    UserId = userId,
                    CatalogDesignId = designId,
                    IsDeleted = false
                };

                await _favoriteRepository.AddAsync(favorite);
            }
            else
            {
                favorite.IsDeleted = !favorite.IsDeleted;
            }

            await _favoriteRepository.SaveChangesAsync();
        }



        public async Task<bool> IsFavoriteAsync(string userId, Guid designId)
        {
            return await _favoriteRepository.ExistsAsync(userId, designId);
        }




    }
}
