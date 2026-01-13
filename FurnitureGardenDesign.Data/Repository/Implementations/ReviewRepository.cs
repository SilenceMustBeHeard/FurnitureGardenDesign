using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class ReviewRepository:
         BaseRepository<Review, Guid>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) :
            base(context)
        {
        }
        public async Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId)
        {
            return await _dbSet
                .AnyAsync(r => r.UserId == userId && r.CatalogDesignId == catalogDesignId);
        }
    }
}

