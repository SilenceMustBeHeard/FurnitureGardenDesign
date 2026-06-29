using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Catalog;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Catalog
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogDesign>> GetAllActiveAsync();

        Task<CatalogDesign?> GetByIdAsync(Guid id);

        Task AddToFavoritesAsync(string userId, Guid designId);

        Task RemoveFromFavoritesAsync(string userId, Guid designId);

        Task AddReviewAsync(string userId, Guid designId, int rating, string? comment);

        Task<IEnumerable<CatalogDesignViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest);

        Task<CatalogDesignViewModel?> GetDetailsAsync(Guid id, string? userId);

        Task<int> GetTotalActiveDesignsAsync();
    }
}