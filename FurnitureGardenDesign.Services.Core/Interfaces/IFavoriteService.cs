using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IFavoriteService
    {

        public Task AddToFavoritesAsync(string userId, Guid catalogDesignId);
        public IEnumerable<Favorite> GetFavoritesByUserId(string userId);
        public Task RemoveFromFavoritesAsync(string userId, Guid catalogDesignId);

    }
}
