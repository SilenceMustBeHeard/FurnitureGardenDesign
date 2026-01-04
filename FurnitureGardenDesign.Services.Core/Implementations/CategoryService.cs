using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryAsync<Category, Guid> _categoryRepo;

        public CategoryService(IRepositoryAsync<Category, Guid> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task CreateCategoryAsync(Category model)
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true
            };

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
        {
            return await _categoryRepo.GetCategoriesAsync();
        }
    }
}