namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(string userId, Guid designId);

        Task<bool> IsFavoriteAsync(string userId, Guid designId);
    }
}