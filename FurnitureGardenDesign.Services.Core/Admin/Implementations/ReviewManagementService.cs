using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations
{
    public class ReviewManagementService : IReviewManagementService
    {
        private readonly IReviewManagementRepository _reviewRepo;
        private readonly ICatalogRepository _catalogRepo;

        public ReviewManagementService(IReviewManagementRepository reviewRepo,
            ICatalogRepository catalogRepo)
        {
            _reviewRepo = reviewRepo;
            _catalogRepo = catalogRepo;
        }

        // Adds a new review for a catalog design by a user
        public async Task AddReviewAsync(string userId, AddReviewViewModel model)
        {
            var review = new Review
            {
                CatalogDesignId = model.CatalogDesignId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
        }

        // Checks if a user has already reviewed a specific catalog design
        public async Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId)
        {
            return await _reviewRepo.HasUserReviewedAsync(userId, catalogDesignId);
        }

        // Retrieves all reviews for a specific catalog design and maps them to view models
        public async Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId)
        {
            var reviews = await _reviewRepo.GetReviewsByDesignIdAsync(catalogDesignId);

            return reviews.Select(r => new AddReviewViewModel
            {
                CatalogDesignId = r.CatalogDesignId,
                CatalogDesignTitle = r.CatalogDesign?.Title ?? "Unknown",
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        // Retrieves a catalog design and its review form
        public async Task<CatalogDesignViewModel?> GetWriteReviewModelAsync(string userId, Guid designId)
        {
            if (await HasUserReviewedAsync(userId, designId))
                return null;

            var design = await _catalogRepo.GetByIdWithReviewsAsync(designId);
            if (design == null)
                return null;

            return new CatalogDesignViewModel
            {
                Id = design.Id,
                Title = design.Title,
                Description = design.Description,
                Image2DUrl = design.Image2DUrl,
                Model3DUrl = design.Model3DUrl,
                Price = design.Price,
                AverageRating = design.Reviews != null && design.Reviews.Any()
                    ? design.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = design.Reviews?.Count ?? 0
            };
        }

        // Creates a new review for a catalog design by a user
        public async Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model)
        {
            if (await HasUserReviewedAsync(userId, model.CatalogDesignId))
                return (false, "You have already reviewed this design.");

            var review = new Review
            {
                CatalogDesignId = model.CatalogDesignId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return (true, null);
        }

        // Gets all active reviews (not deleted) with user and design information
        public async Task<IEnumerable<ReviewViewModelList>> GetAllActiveAsync()
        {
            var reviews = await _reviewRepo
                .GetAllAttached()
                .Where(r => !r.IsDeleted)
                .Include(r => r.User)
                .Include(r => r.CatalogDesign)
                .Select(r => new ReviewViewModelList
                {
                    Id = r.Id,
                    CatalogDesignId = r.CatalogDesignId,
                    CatalogDesignTitle = r.CatalogDesign != null ? r.CatalogDesign.Title : "Unknown",
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                    UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedOn,
                    IsDeleted = r.IsDeleted
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews;
        }

        // Gets all reviews including deleted ones
        public async Task<IEnumerable<ReviewViewModelList>> GetAllIncludingDeletedAsync()
        {
            var reviews = await _reviewRepo
                .GetAllAttached()
                .Include(r => r.User)
                .Include(r => r.CatalogDesign)
                .Select(r => new ReviewViewModelList
                {
                    Id = r.Id,
                    CatalogDesignId = r.CatalogDesignId,
                    CatalogDesignTitle = r.CatalogDesign != null ? r.CatalogDesign.Title : "Unknown",
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                    UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedOn,
                    IsDeleted = r.IsDeleted
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews;
        }

        // Gets a specific review by ID
        public async Task<ReviewViewModelList?> GetByIdAsync(Guid id)
        {
            return await _reviewRepo
                .GetAllAttached()
                .Where(r => r.Id == id)
                .Include(r => r.User)
                .Include(r => r.CatalogDesign)
                .Select(r => new ReviewViewModelList
                {
                    Id = r.Id,
                    CatalogDesignId = r.CatalogDesignId,
                    CatalogDesignTitle = r.CatalogDesign != null ? r.CatalogDesign.Title : "Unknown",
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                    UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedOn,
                    IsDeleted = r.IsDeleted
                })
                .FirstOrDefaultAsync();
        }

        // Gets total count of active reviews
        public async Task<int> GetTotalActiveReviewsAsync()
        {
            return await _reviewRepo
                .GetAllAttached()
                .Where(r => !r.IsDeleted)
                .CountAsync();
        }

        // Toggles the active/deleted status of a review (soft delete/restore)
        public async Task ToggleReviewAsync(Guid id)
        {
            var review = await _reviewRepo.GetByIdIncludingDeletedAsync(id);

            if (review == null)
            {
                throw new Exception($"Review with ID {id} not found");
            }

            await _reviewRepo.ToggleReviewStatusAsync(review);
            await _reviewRepo.SaveChangesAsync();
        }

        // Permanently deletes a review (hard delete - use with caution)
        //public async Task<bool> HardDeleteReviewAsync(Guid id)
        //{
        //    var review = await _reviewRepo.GetByIdIncludingDeletedAsync(id);

        //    if (review == null)
        //        return false;

        //    _reviewRepo.Remove(review);
        //    await _reviewRepo.SaveChangesAsync();
        //    return true;
        //}

        // Gets reviews by user ID
        public async Task<IEnumerable<ReviewViewModelList>> GetReviewsByUserIdAsync(string userId)
        {
            return await _reviewRepo
                .GetAllAttached()
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .Include(r => r.CatalogDesign)
                .Select(r => new ReviewViewModelList
                {
                    Id = r.Id,
                    CatalogDesignId = r.CatalogDesignId,
                    CatalogDesignTitle = r.CatalogDesign != null ? r.CatalogDesign.Title : "Unknown",
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                    UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedOn,
                    IsDeleted = r.IsDeleted
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Gets reviews by catalog design ID with full details
        public async Task<IEnumerable<ReviewViewModelList>> GetDetailedReviewsByDesignIdAsync(Guid catalogDesignId)
        {
            return await _reviewRepo
                .GetAllAttached()
                .Where(r => r.CatalogDesignId == catalogDesignId && !r.IsDeleted)
                .Include(r => r.User)
                .Include(r => r.CatalogDesign)
                .Select(r => new ReviewViewModelList
                {
                    Id = r.Id,
                    CatalogDesignId = r.CatalogDesignId,
                    CatalogDesignTitle = r.CatalogDesign != null ? r.CatalogDesign.Title : "Unknown",
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                    UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedOn,
                    IsDeleted = r.IsDeleted
                })
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        //// Updates an existing review
        //public async Task<(bool Success, string? Error)> UpdateReviewAsync(Guid reviewId, string userId, int rating, string? comment)
        //{
        //    var review = await _reviewRepo.GetByIdAsync(reviewId);

        //    if (review == null)
        //        return (false, "Review not found");

        //    if (review.UserId != userId)
        //        return (false, "You can only edit your own reviews");

        //    review.Rating = rating;
        //    review.Comment = comment ?? string.Empty;
        

        //    _reviewRepo.Update(review);
        //    await _reviewRepo.SaveChangesAsync();

        //    return (true, null);
        //}

        // Gets average rating for a design
        public async Task<double> GetAverageRatingForDesignAsync(Guid catalogDesignId)
        {
            var reviews = await _reviewRepo
                .GetAllAttached()
                .Where(r => r.CatalogDesignId == catalogDesignId && !r.IsDeleted)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        // Gets review count for a design
        public async Task<int> GetReviewCountForDesignAsync(Guid catalogDesignId)
        {
            return await _reviewRepo
                .GetAllAttached()
                .Where(r => r.CatalogDesignId == catalogDesignId && !r.IsDeleted)
                .CountAsync();
        }
    }
}