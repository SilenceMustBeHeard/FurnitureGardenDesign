using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using FurnitureGardenDesign.Web.ViewModels.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly ICatalogRepository _catalogRepo;

        public ReviewService(IReviewRepository reviewRepo, ICatalogRepository catalogRepo)
        {
            _reviewRepo = reviewRepo;
            this._catalogRepo = catalogRepo;

        }


        // adds a new review for a catalog design by a user
        public async Task AddReviewAsync(string userId, AddReviewViewModel model)
        {
            var review = new Review
            {
                CatalogDesignId = model.CatalogDesignId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
        }

        // checks if a user has already reviewed a specific catalog design
        public async Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId)
        {
            return await _reviewRepo.HasUserReviewedAsync(userId, catalogDesignId);
        }


        // retrieves all reviews for a specific catalog design and maps them to view models
        //  allows the application to display reviews for a catalog design including the rating and comment left by users
        public async Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId)
        {
            var reviews = await _reviewRepo.GetReviewsByDesignIdAsync(catalogDesignId);

            return reviews.Select(r => new AddReviewViewModel
            {
                CatalogDesignId = r.CatalogDesignId,
                CatalogDesignTitle = r.CatalogDesign.Title,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }

        // retrieves a catalog design and its review form 
        public async Task<CatalogDesignViewModel?> GetWriteReviewModelAsync(string userId, Guid designId)
        {
            if (await HasUserReviewedAsync(userId, designId))
                return null;

            var design = await _catalogRepo.GetByIdWithReviewsAsync(designId);
            if (design == null)
                return null;
            // maps the catalog design and its reviews to a view model that includes the average rating and review count
            return new CatalogDesignViewModel
            {
                Id = design.Id,
                Title = design.Title,
                Description = design.Description,
                Image2DUrl = design.Image2DUrl,
                Model3DUrl = design.Model3DUrl,
                Price = design.Price,
                AverageRating = design.Reviews.Any()
                    ? design.Reviews.Average(r => r.Rating)
                    : 0,
                ReviewCount = design.Reviews.Count
            };
        }




        // creates a new review for a catalog design by a user
        // ensuring that the user has not already reviewed the design
        public async Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model)
        {
            if (await HasUserReviewedAsync(userId, model.CatalogDesignId))
                return (false, "You have already reviewed this design.");

            var review = new Review
            {
                CatalogDesignId = model.CatalogDesignId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return (true, null);
        }

    }
}