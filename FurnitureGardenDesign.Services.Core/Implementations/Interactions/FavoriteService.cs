using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using FurnitureGardenDesign.Services.Core.Interfaces;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }


    // Toggles the favorite status of a design for a user.
    // If the design is not currently a favorite it will be added. If it is already a favorite it will be removed.
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
        // toggle logic

        favorite.IsDeleted = !favorite.IsDeleted;

        await _favoriteRepository.SaveChangesAsync();

       
        return !favorite.IsDeleted;
    }

    // Checks if a design is marked as a favorite by a user
    public async Task<bool> IsFavoriteAsync(string userId, Guid designId)
    {
        return await _favoriteRepository.ExistsAsync(userId, designId);
    }
}
