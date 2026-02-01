using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepo;
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly IReviewRepository _reviewRepo;

        public CatalogService(
            ICatalogRepository catalogRepo,
            IFavoriteRepository favoriteRepo,
            IReviewRepository reviewRepo)
        {
            _catalogRepo = catalogRepo;
            _favoriteRepo = favoriteRepo;
            _reviewRepo = reviewRepo;
        }


        public async Task<IEnumerable<CatalogDesign>> GetAllActiveAsync()
      => await _catalogRepo
          .GetAllAttached()
          .Where(c => c.IsActive)
          .Include(c => c.Category)
          .Include(c => c.Materials)
          .Include(c => c.Reviews)
          .Include(c => c.Favorites)

          .ToListAsync();



        public async Task<CatalogDesign?> GetByIdAsync(Guid id)
        {
            return await _catalogRepo
                .GetAllAttached()
                .Where(c => c.IsActive)
                .Include(c => c.Category)
                .Include(c => c.Materials)
                .Include(c => c.Reviews)
                .Include(c => c.Favorites)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task AddToFavoritesAsync(string userId, Guid designId)
        {
            bool exists = await _favoriteRepo
                .GetAllAttached()
                .AnyAsync(f => f.UserId == userId && f.CatalogDesignId == designId);

            if (!exists)
            {
                await _favoriteRepo.AddAsync(new Favorite
                {
                    UserId = userId,
                    CatalogDesignId = designId
                });

                await _favoriteRepo.SaveChangesAsync();
            }
        }

        public async Task RemoveFromFavoritesAsync(string userId, Guid designId)
        {
            var favorite = await _favoriteRepo
                .GetAllAttached()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CatalogDesignId == designId);

            if (favorite != null)
            {
                await _favoriteRepo.DeleteAsync(favorite);
                await _favoriteRepo.SaveChangesAsync();
            }
        }



        public async Task AddReviewAsync(string userId, Guid designId, int rating, string? comment)
        {
            var review = new Review
            {
                CatalogDesignId = designId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };
            await _reviewRepo.AddAsync(review);
        }

        public async Task AddReviewsAsync(string userId, Guid designId, int rating, string? comment)
        {
            var review = new Review
            {
                CatalogDesignId = designId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
        }

        






        public async Task<IEnumerable<CatalogDesignViewModel>> GetPublicCatalogAsync()
        
            => await _catalogRepo
                .GetAllAttached()
                .Where(d => d.IsActive)
                .Select(d => new CatalogDesignViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    ImageUrl = d.ImageUrl,
                    Price = d.Price,
                    AverageRating = d.Reviews.Any()
                        ? d.Reviews.Average(r => r.Rating)
                        : 0,
                    ReviewCount = d.Reviews.Count
                })
                .ToListAsync();
        


    }
}


