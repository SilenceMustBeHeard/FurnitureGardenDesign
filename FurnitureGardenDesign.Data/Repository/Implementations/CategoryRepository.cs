using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{
    public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        // Only active 
        public async Task<IEnumerable<Category>> GetAllActiveAsync()
            => await _dbSet.Where(c => c.IsDeleted).ToListAsync();

        // All categories for admin
        public async Task<IEnumerable<Category>> GetAllForAdminAsync()
            => await _dbSet.IgnoreQueryFilters().ToListAsync();

        public Category? GetByName(string name)
            => _dbSet.FirstOrDefault(c => c.Name == name);

        // Toggle IsDeleted status (soft-delete / restore)
        public async Task ToggleCategoryStatusAsync(Category category)
        {
            await ToggleStatusAsync(category);
        }

        // Get by ID including deleted
        public async Task<Category?> GetByIdIncludingDeletedAsync(Guid id)
        {
            return await _dbSet.IgnoreQueryFilters()
                               .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
