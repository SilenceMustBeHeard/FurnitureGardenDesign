using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions
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



        public async Task<IEnumerable<Review>> GetReviewsByDesignIdAsync(Guid catalogDesignId)
        {
            
            return await _dbSet
                .Include(r => r.CatalogDesign)
                .Where(r => r.CatalogDesignId == catalogDesignId)
                .ToListAsync();
        }
    }
}

