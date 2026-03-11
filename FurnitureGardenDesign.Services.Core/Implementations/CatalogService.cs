using FurnitureGardenDesign.Data.Common.Enums;
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

        // gets all active catalog designs with related data for admin panel or internal use
        public async Task<IEnumerable<CatalogDesign>> GetAllActiveAsync()
      => await _catalogRepo
          .GetAllAttached()
          .Where(c => c.IsActive)
          .Include(c => c.Category)
        
          .Include(c => c.Reviews)
          .Include(c => c.Favorites)

          .ToListAsync();


        // For admin panel or internal use
        public async Task<CatalogDesign?> GetByIdAsync(Guid id)
        {
            return await _catalogRepo
                .GetAllAttached()
                .Where(c => c.IsActive)
                .Include(c => c.Category)
               
                .Include(c => c.Reviews)
                .Include(c => c.Favorites)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Add to favorites
        public async Task AddToFavoritesAsync(string userId, Guid designId)
        {
            // Check if the about-to favorite and user  exists
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

        // Soft delete(removes) from favorites
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


        // Add review
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
            await _reviewRepo.SaveChangesAsync();
        }







        // For public catalog listing with pagination
        public async Task<IEnumerable<CatalogDesignViewModel>> GetPublicCatalogAsync(
         string? userId,
         int page,
         int pageSize,
         bool isGuest)
        {
            var query = _catalogRepo
                .GetAllAttached()
                .Where(d => d.IsActive);

            if (isGuest)
            {
                page = 1;
                pageSize = 3;
            }

            return await query
                .OrderByDescending(d => d.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new CatalogDesignViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Image2DUrl = d.Image2DUrl,

                    Model3DUrl = !string.IsNullOrWhiteSpace(d.Model3DUrl)
                        ? d.Model3DUrl
                        : null,

                    Model3DStatus = !string.IsNullOrWhiteSpace(d.Model3DUrl)
                        ? Model3DStatus.Ready
                        : Model3DStatus.None,

                    Price = d.Price,
                    CategoryName = d.Category.Name,

                    IsFavorited = userId != null &&
                        d.Favorites.Any(f => f.UserId == userId && !f.IsDeleted),

                    AverageRating = d.Reviews.Any()
                        ? d.Reviews.Average(r => r.Rating)
                        : 0,

                    ReviewCount = d.Reviews.Count
                })
                .ToListAsync();
        }



        // get total count for pagination
        public async Task<int> GetTotalActiveDesignsAsync()
        {
            return await _catalogRepo
                .GetAllAttached()
                .Where(d => d.IsActive)
                .CountAsync();
        }


        // For details page
        public async Task<CatalogDesignViewModel?> GetDetailsAsync(
    Guid id,
    string? userId)
        {
            return await _catalogRepo
                .GetAllAttached()
                .Where(d => d.Id == id && d.IsActive)
           .Select(d => new CatalogDesignViewModel
           {
               Id = d.Id,
               Title = d.Title,
               Description = d.Description,
               Image2DUrl = d.Image2DUrl,
               // Only return Model3DUrl if it exists, otherwise null
               // used to control frontend rendering and loading of 3D model
               Model3DUrl = !string.IsNullOrWhiteSpace(d.Model3DUrl)
               ? d.Model3DUrl
               : null,
               // Determine 3D model status based on presence of URL
               Model3DStatus = !string.IsNullOrWhiteSpace(d.Model3DUrl)
              ? Model3DStatus.Ready
               : Model3DStatus.None,

               Price = d.Price,
               CategoryName = d.Category.Name,
               // Check if the current user has favorited this design
               IsFavorited = userId != null &&
        d.Favorites.Any(f => f.UserId == userId && !f.IsDeleted),
               // Calculate average rating, default to 0 if no reviews
               AverageRating = d.Reviews.Any()
        ? d.Reviews.Average(r => r.Rating)
        : 0,

               ReviewCount = d.Reviews.Count
           })
              .FirstOrDefaultAsync();

            
        }




    }
}


