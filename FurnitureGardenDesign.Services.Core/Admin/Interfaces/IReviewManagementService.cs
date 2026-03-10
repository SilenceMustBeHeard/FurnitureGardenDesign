using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Review;


namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IReviewManagementService
    {
        // Basic CRUD operations
        Task AddReviewAsync(string userId, AddReviewViewModel model);
        Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model);
        //Task<(bool Success, string? Error)> UpdateReviewAsync(Guid reviewId, string userId, int rating, string? comment);
        //Task<bool> HardDeleteReviewAsync(Guid id);

        // Query operations
        Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId);
        Task<IEnumerable<ReviewViewModelList>> GetDetailedReviewsByDesignIdAsync(Guid catalogDesignId);
        Task<IEnumerable<ReviewViewModelList>> GetReviewsByUserIdAsync(string userId);
        Task<ReviewViewModelList?> GetByIdAsync(Guid id);

        // Status management
        Task ToggleReviewAsync(Guid id);
        Task<IEnumerable<ReviewViewModelList>> GetAllActiveAsync();
        Task<IEnumerable<ReviewViewModelList>> GetAllIncludingDeletedAsync();

        // Validation
        Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId);

        // Statistics
        Task<int> GetTotalActiveReviewsAsync();
        Task<double> GetAverageRatingForDesignAsync(Guid catalogDesignId);
        Task<int> GetReviewCountForDesignAsync(Guid catalogDesignId);

        // Write review model
        Task<CatalogDesignViewModel?> GetWriteReviewModelAsync(string userId, Guid designId);
    }
}