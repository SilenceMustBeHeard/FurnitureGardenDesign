using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Implementations;
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






        public async Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId)
        {
            return await _reviewRepo.HasUserReviewedAsync(userId, catalogDesignId);
        }


    }

}
