using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogDesign>> GetAllActiveAsync();
        Task<CatalogDesign?> GetByIdAsync(Guid id);
        Task AddToFavoritesAsync(string userId, Guid designId);
        Task RemoveFromFavoritesAsync(string userId, Guid designId);
        Task AddReviewAsync(string userId, Guid designId, int rating, string? comment);
        Task<IEnumerable<Review>> GetReviewsAsync(Guid designId);
        Task<IEnumerable<CatalogDesignViewModel>> GetPublicCatalogAsync();
    }
}
