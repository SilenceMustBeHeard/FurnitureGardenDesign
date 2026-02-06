using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface ICategoryRepository:
         IRepository<Category, Guid>, IRepositoryAsync<Category, Guid>
    {
        Task<Category?> GetByIdIncludingDeletedAsync(Guid id);
        Task<IEnumerable<Category>> GetAllActiveAsync();

        Task<IEnumerable<Category>> GetAllForAdminAsync();
        Task ToggleCategoryStatusAsync(Category category);
            Category? GetByName(string name);


    }
}
