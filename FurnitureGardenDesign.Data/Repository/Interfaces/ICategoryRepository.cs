using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface ICategoryRepository:
         IRepository<Category, Guid>, IRepositoryAsync<Category, Guid>
    {
        Task<IEnumerable<Category>> GetAllActiveAsync();
        Category? GetByName(string name);
    }
}
