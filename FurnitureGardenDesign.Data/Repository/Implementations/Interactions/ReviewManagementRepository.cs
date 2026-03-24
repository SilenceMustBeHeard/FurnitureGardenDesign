using FurnitureGardenDesign.Data.Models.Interactions;
using FurnitureGardenDesign.Data.Repository.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions
{
    public class ReviewManagementRepository : ReviewRepository, IReviewManagementRepository
    {
        public ReviewManagementRepository(ApplicationDbContext context) :
            base(context)
        {
        }

        // returns active reviews
        public async Task<IEnumerable<Review>> GetAllActiveAsync()
            => await _dbSet
                .Where(r => !r.IsDeleted)  
                .Include(r => r.CatalogDesign)
                .Include(r => r.User)
                .ToListAsync();

        // includes related data for admin view
        public async Task<IEnumerable<Review>> GetAllForAdminAsync()
            => await _dbSet
                .IgnoreQueryFilters()
                .Include(r => r.CatalogDesign)  
                .Include(r => r.User)            
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

      

        public async Task ToggleReviewStatusAsync(Review review)
        {
            review.IsDeleted = !review.IsDeleted;  
            _dbSet.Update(review);
            await _context.SaveChangesAsync();
        }

        
        public async Task<Review?> GetByIdIncludingDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(r => r.CatalogDesign)
                .Include(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Hard delete a review permanently from the database
        public async Task<bool> HardDeleteReviewAsync(Guid id)
        {
            var review = await _dbSet.FindAsync(id);
            if (review == null) return false;

            _dbSet.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}