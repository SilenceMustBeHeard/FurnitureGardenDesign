using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Review;
using Microsoft.AspNetCore.Identity;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task AddReviewAsync(string userId, AddReviewViewModel model)
        {
            if (model.Rating < 0 
                || model.Rating > 5)

            {
                throw new ArgumentOutOfRangeException(nameof(model.Rating));
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CatalogDesignId = model.CatalogDesignId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedOn = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);
        }
    }

}
