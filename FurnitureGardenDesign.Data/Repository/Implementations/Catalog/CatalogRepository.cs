using FurnitureGardenDesign.Data.Models.Catalog;
using FurnitureGardenDesign.Data.Repository.Implementations.Account;
using FurnitureGardenDesign.Data.Repository.Interfaces.Catalog;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Catalog
{
    public class CatalogRepository
        : BaseRepository<CatalogDesign, Guid>, ICatalogRepository
    {
        public CatalogRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<CatalogDesign>> GetAllActiveAsync()
            => await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Category)
                .ToListAsync();

        public async Task<CatalogDesign?> GetByIdWithReviewsAsync(Guid id)
            => await GetAllAttached()
                .Include(c => c.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<CatalogDesign>> GetAllForAdminAsync()
            => await _dbSet
                .IgnoreQueryFilters()
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();

        public CatalogDesign? GetByName(string name)
            => _dbSet.FirstOrDefault(c => c.Title == name);

        public async Task ToggleCatalogStatusAsync(CatalogDesign catalog)
        {
            catalog.IsDeleted = !catalog.IsDeleted;
            _dbSet.Update(catalog);
            await _context.SaveChangesAsync();
        }

        public async Task<CatalogDesign?> GetByIdIncludingDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}