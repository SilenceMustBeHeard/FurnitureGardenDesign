using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{


    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll();              
        Task<List<Category>> GetAllActiveAsync();   
        Task<Category?> GetByIdAsync(Guid id);
        Task AddAsync(Category category);
        Task<bool> UpdateAsync(Category category);
    }

}
