using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class CatalogService : ICatalogService
    {
        private readonly IRepositoryAsync<CatalogDesign, Guid> _catalogRepo;

        public CatalogService(IRepositoryAsync<CatalogDesign, Guid> catalogRepo)
        {
            _catalogRepo = catalogRepo;
        }

        public async Task<IEnumerable<CatalogDesign>> GetAllDesignsAsync()
        {
            return await _catalogRepo.GetCategoriesAsync(); 
        }

        public async Task<CatalogDesign?> GetDesignByIdAsync(Guid id)
        {
            return await _catalogRepo.GetByIdAsync(id);
        }
    }

}
