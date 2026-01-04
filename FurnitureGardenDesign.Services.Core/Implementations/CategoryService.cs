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
        private readonly IRepositoryAsync<Category, Guid> _repo;

        public CategoryService(IRepositoryAsync<Category, Guid> repo)
        {
            _repo = repo;
        }

        public async Task CreateCategoryAsync(Category model)
        {
            var category = new Category
            {
                Name = model.Name,
                IsActive = true
            };

            await _repo.AddAsync(category);
        }

        public Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
       => _repo.GetCategoriesAsync();
    }

}