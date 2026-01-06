using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations
{

    public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAllActiveAsync()
            => await _dbSet.Where(c => c.IsActive).ToListAsync();

        public Category? GetByName(string name)
            => _dbSet.FirstOrDefault(c => c.Name == name);
    }


}
