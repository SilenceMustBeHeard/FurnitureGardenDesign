using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<bool> ToggleFavoriteAsync(string userId, Guid designId)
    {
        var favorite = await _favoriteRepository.GetByCompositeKeyAsync(userId, designId);

        if (favorite == null)
        {
            await _favoriteRepository.AddAsync(new Favorite
            {
                UserId = userId,
                CatalogDesignId = designId,
                IsDeleted = false
            });

           
            return true;
        }

        favorite.IsDeleted = !favorite.IsDeleted;

        await _favoriteRepository.SaveChangesAsync();

       
        return !favorite.IsDeleted;
    }


    public async Task<bool> IsFavoriteAsync(string userId, Guid designId)
    {
        return await _favoriteRepository.ExistsAsync(userId, designId);
    }
}
